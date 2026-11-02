using System.ComponentModel;
using System.Diagnostics;

namespace SdiConverter;

public partial class MultiChannelForm : Form
{
    private readonly ChannelConfiguration configuration;
    private readonly List<ChannelControl> channels = new();
    private readonly SystemLoad load = new();
    private readonly System.Windows.Forms.Timer timer = new() { Interval = 1000 };
    private bool closing, closeReady, refreshing, batch, cancelBatch;
    private int highLoadSamples;

    public MultiChannelForm()
    {
        InitializeComponent();
        configuration = new ChannelConfiguration();
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
        configuration = ChannelConfiguration.Load();
        InitializeRuntime();
    }

    public MultiChannelForm(ChannelConfiguration configuration) : this()
    {
        this.configuration = configuration;
    }

    private void InitializeRuntime()
    {
        Text = $"SDI Converter 2 | {GetType().Assembly.GetName().Version} | Peter Aellig";
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        Shown += async (_, _) => await RefreshPortsAsync();
        refreshButton.Click += async (_, _) => await RefreshPortsAsync();
        startAllButton.Click += async (_, _) =>
        {
            if (batch || refreshing || closing) return;
            batch = true; cancelBatch = false; UpdateButtons();
            try
            {
                // Stagger startup and SDK enumeration; running channels remain independent.
                foreach (var channel in channels)
                {
                    if (closing || cancelBatch) break;
                    if (!channel.Active) await channel.StartAsync();
                }
            }
            finally { batch = false; UpdateButtons(); }
        };
        stopAllButton.Click += async (_, _) =>
        {
            if (refreshing || closing || cancelBatch) return;
            cancelBatch = true;
            stopAllButton.Enabled = false;
            while (batch) await Task.Delay(25);
            batch = true; UpdateButtons();
            try { await Task.WhenAll(channels.Select(c => c.StopAsync())); }
            finally { batch = false; cancelBatch = false; UpdateButtons(); }
        };
        saveButton.Click += (_, _) => SaveSettings();
        overview.SelectionChanged += (_, _) =>
        {
            if (overview.CurrentRow is { Index: var index } && index < tabs.TabCount) tabs.SelectedIndex = index;
        };
        linkLabel1.LinkClicked += (_, _) =>
        {
            try
            {
                Process.Start(new ProcessStartInfo(Path.Combine(AppContext.BaseDirectory, "OMT-LICENSE.txt")) { UseShellExecute = true });
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
            {
                MessageBox.Show(this, "Could not open the OMT license: " + ex.Message, "Open Media Transport", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        };
        footer.LinkClicked += (_, _) => Process.Start(new ProcessStartInfo("https://ndi.video/") { UseShellExecute = true });
        footer.LinkArea = new LinkArea(footer.Text.IndexOf("https://", StringComparison.Ordinal), "https://ndi.video/".Length);
        FormClosing += CloseAsync;
        timer.Tick += (_, _) => SampleLoad(); timer.Start();
        FormClosed += (_, _) => { timer.Dispose(); foreach (var c in channels) c.CloseDiagnostics(); };
    }

    private void UpdateButtons()
    {
        refreshButton.Enabled = !closing && !refreshing && !batch && channels.All(c => !c.Active);
        startAllButton.Enabled = !closing && !refreshing && !batch && channels.Count > 0;
        stopAllButton.Enabled = !closing && !refreshing && channels.Count > 0 && !cancelBatch;
        saveButton.Enabled = !closing && !refreshing;
    }

    private async Task RefreshPortsAsync()
    {
        if (refreshing || closing || channels.Any(c => c.Active)) return;
        refreshing = true; UpdateButtons();
        try
        {
            var devices = (await SdiDevice.DiscoverAsync()).Where(d => d.Capture || d.Playback).ToList();
            if (closing) return;
            if (devices.Select(d => d.Key).Distinct().Count() != devices.Count)
                throw new InvalidOperationException("The SDK returned ambiguous port identities. Check the card configuration.");
            if (channels.Count > 0) SaveSettings();
            foreach (var channel in channels) { channel.CloseDiagnostics(); }
            foreach (var page in tabs.TabPages.Cast<TabPage>().ToArray()) page.Dispose();
            channels.Clear(); overview.Rows.Clear();
            var suggestedOutput = devices.LastOrDefault(d => d.Playback && d.Busy == 0);
            foreach (var device in devices)
            {
                var settings = configuration.Channels.GetValueOrDefault(device.Key) ?? new ChannelSettings
                {
                    Direction = !device.Capture || device == suggestedOutput && devices.Count > 1 ? "Output" : "Input",
                    Sender = $"SDI Converter 2 - Port {device.Index + 1}"
                };
                var channel = new ChannelControl(device, settings) { Dock = DockStyle.Top };
                var page = new TabPage($"Port {device.Index + 1} · {device.Name}") { AutoScroll = true };
                page.Controls.Add(channel); tabs.TabPages.Add(page); channels.Add(channel);
                overview.Rows.Add(device.Name, channel.Direction, channel.Status, "—", "—");
                channel.Changed += () =>
                {
                    UpdateRow(channel); UpdateButtons();
                    detectionLabel.Text = $"{devices.Count} SDK ports detected · {devices.Count(d => d.Busy == 0)} free at last scan · " +
                        $"{channels.Count(c => c.Direction == "Input")} input / {channels.Count(c => c.Direction == "Output")} output configured.";
                };
                channel.SettingsChanged += SaveSettings;
            }
            var available = devices.Count(d => d.Busy == 0);
            var inputs = channels.Count(c => c.Direction == "Input");
            detectionLabel.Text = devices.Count == 0 ? "No active SDI ports found. Check Desktop Video Setup, then refresh."
                : $"{devices.Count} SDK ports detected · {available} free · {inputs} input / {channels.Count - inputs} output configured. Change direction per port below.";
        }
        catch (Exception ex) { detectionLabel.Text = "Port detection failed: " + ex.Message; }
        finally { refreshing = false; if (!IsDisposed) UpdateButtons(); }
    }

    private void UpdateRow(ChannelControl channel)
    {
        var index = channels.IndexOf(channel);
        if (index < 0 || index >= overview.Rows.Count) return;
        var row = overview.Rows[index];
        row.Cells[1].Value = channel.Direction; row.Cells[2].Value = channel.Status;
        row.Cells[3].Value = channel.Cpu; row.Cells[4].Value = channel.Ram;
        row.DefaultCellStyle.BackColor = channel.HasRecentLoss ? Color.MistyRose : SystemColors.Window;
    }

    private void SampleLoad()
    {
        var sample = load.Read(null);
        foreach (var channel in channels) { channel.SampleLoad(); UpdateRow(channel); }
        highLoadSamples = load.LastCpuPercent >= 85 ? highLoadSamples + 1 : 0;
        loadLabel.Text = $"System CPU {sample.Cpu} · RAM {sample.Ram} · free {sample.FreeRam}       All channels: CPU {channels.Sum(c => c.CpuPercent):0.0}% · RAM {channels.Sum(c => c.RamBytes) / 1048576.0:0} MB";
        warningLabel.Text = channels.Any(c => c.HasRecentLoss) ? "New frame/audio errors detected. Check the affected channel, source and network."
            : highLoadSamples >= 5 ? "System CPU above 85% for at least 5 seconds. Little reserve for additional streams."
            : "CPU values refer to the whole CPU. Port count is not a guarantee of processing capacity.";
        warningLabel.ForeColor = channels.Any(c => c.HasRecentLoss) || highLoadSamples >= 5 ? Color.Firebrick : SystemColors.ControlText;
    }

    private void SaveSettings()
    {
        foreach (var channel in channels) configuration.Channels[channel.Key] = channel.Save();
        try { configuration.Save(); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { detectionLabel.Text = "Could not save settings: " + ex.Message; }
    }

    private async void CloseAsync(object? sender, FormClosingEventArgs e)
    {
        if (closeReady) return;
        e.Cancel = true;
        if (closing) return;
        if (e.CloseReason is not (CloseReason.WindowsShutDown or CloseReason.TaskManagerClosing or CloseReason.ApplicationExitCall) &&
            MessageBox.Show(this, channels.Any(c => c.Active) ? "Stop all channels and exit SDI Converter 2?" : "Exit SDI Converter 2?",
                Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
        closing = true; UpdateButtons(); tabs.Enabled = false;
        // Allow an in-flight discovery/start operation to settle before stopping its process.
        while (batch || refreshing) await Task.Delay(25);
        await Task.WhenAll(channels.Select(c => c.StopAsync()));
        if (channels.Any(c => c.Active))
        {
            closing = false; tabs.Enabled = true; UpdateButtons();
            MessageBox.Show(this, "Some channels could not stop. Check their status before closing."); return;
        }
        SaveSettings(); timer.Stop(); closeReady = true; Close();
    }
}
