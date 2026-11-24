using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace SdiConverter;

public partial class ChannelControl : UserControl
{
    private SdiDevice? device;
    private EngineProcess? engine;
    private bool transitioning, stopping, discovering;
    private string lastError = "";
    private readonly TextBox log = new() { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both, Font = new Font("Consolas", 9) };
    private DiagnosticsForm? diagnostics;
    private readonly SystemLoad load = new();
    private long previousLoss;
    private bool recentLoss;
    // Automatic restart after a failure: waits 3 s, doubling up to 30 s while the engine keeps failing quickly.
    private CancellationTokenSource? restartPending;
    private int restartDelay;
    private long engineStartedAt;
    public event Action? Changed;
    public event Action? SettingsChanged;
    public bool Active => engine != null || transitioning || restartPending != null;
    public bool AutoStart => autoStartBox.Checked;
    public string Key => device!.Key;
    public string DeviceName => device!.Name;
    public string Direction => directionBox.Text;
    public string Status => statusLabel.Text;
    public string Cpu { get; private set; } = "—";
    public string Ram { get; private set; } = "—";
    public double CpuPercent { get; private set; }
    public long RamBytes { get; private set; }
    public bool HasRecentLoss => Active && recentLoss;

    public ChannelControl()
    {
        InitializeComponent();
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
        directionBox.SelectedIndexChanged += (_, _) => UpdateInputs();
        kindBox.SelectedIndexChanged += (_, _) => { sourceBox.Items.Clear(); UpdateInputs(); };
        outputBox.SelectedIndexChanged += (_, _) => UpdateInputs();
        startButton.Click += async (_, _) => { if (Active) await StopAsync(); else await StartAsync(); };
        findButton.Click += async (_, _) => await FindAsync();
        logButton.Click += (_, _) =>
        {
            if (diagnostics is { IsDisposed: false }) { diagnostics.Activate(); return; }
            diagnostics = new DiagnosticsForm(DeviceName, log); diagnostics.Show(FindForm());
        };
    }

    public ChannelControl(SdiDevice device, ChannelSettings settings) : this()
    {
        this.device = device;
        portLabel.Text = device.Name;
        if (device.Capture) directionBox.Items.Add("Input");
        if (device.Playback) directionBox.Items.Add("Output");
        Select(directionBox, settings.Direction);
        senderBox.Text = settings.Sender;
        sourceBox.Text = settings.Source;
        Select(outputBox, settings.Output); Select(kindBox, settings.SourceKind);
        Select(qualityBox, settings.Quality); Select(audioBox, settings.Audio); Select(duplicateBox, settings.Duplicate);
        bufferBox.Value = Math.Clamp(settings.Buffer, 2, 8);
        ndiLevelBox.Checked = settings.NdiReferenceLevel;
        autoRestartBox.Checked = settings.AutoRestart;
        autoStartBox.Checked = settings.AutoStart;
        statusLabel.Text = device.Busy is > 0 ? "Port busy in another application" : device.Busy is null ? "Port availability unknown" : "Ready";
        UpdateInputs();
    }

    private static void Select(ComboBox box, string value) => box.SelectedItem = box.Items.Contains(value) ? value : box.Items.Cast<object>().FirstOrDefault();
    public ChannelSettings Save() => new()
    {
        Direction = Direction,
        Sender = senderBox.Text.Trim(),
        Output = outputBox.Text,
        SourceKind = kindBox.Text,
        Source = sourceBox.Text.Trim(),
        Quality = qualityBox.Text,
        Audio = audioBox.Text,
        Duplicate = duplicateBox.Text,
        Buffer = (int)bufferBox.Value,
        NdiReferenceLevel = ndiLevelBox.Checked,
        AutoRestart = autoRestartBox.Checked,
        AutoStart = autoStartBox.Checked
    };

    private void UpdateInputs()
    {
        bool idle = !Active && !discovering, input = Direction == "Input";
        directionBox.Enabled = idle;
        senderBox.Enabled = outputBox.Enabled = qualityBox.Enabled = duplicateBox.Enabled = idle && input;
        kindBox.Enabled = sourceBox.Enabled = findButton.Enabled = bufferBox.Enabled = idle && !input;
        audioBox.Enabled = idle;
        ndiLevelBox.Enabled = idle && (input ? outputBox.Text.Contains("NDI") : kindBox.Text == "NDI");
        startButton.Enabled = !transitioning && !discovering && directionBox.Items.Count > 0;
        startButton.Text = Active ? "Stop channel" : "Start channel";
        Changed?.Invoke();
    }

    public async Task StartAsync() { restartDelay = 0; await StartAsync(false); }

    private async Task StartAsync(bool automatic)
    {
        if (Active || discovering || device is null) return;
        transitioning = true; UpdateInputs();
        try
        {
            var settings = Save();
            if (settings.Direction == "Input" && settings.Sender.Length == 0) throw new InvalidOperationException("Enter a sender name.");
            if (settings.Direction == "Output" && settings.Source.Length == 0) throw new InvalidOperationException("Choose or type a network source.");
            statusLabel.Text = "Checking port availability …";
            var current = (await SdiDevice.DiscoverAsync()).SingleOrDefault(d => d.Key == Key);
            if (current is null || current.Name != device.Name) throw new InvalidOperationException("Port changed or disconnected. Refresh ports.");
            if (current.Busy is > 0) throw new InvalidOperationException("Port is busy in another application.");
            if (current.Busy is null) throw new InvalidOperationException("Cannot verify port availability. Check Desktop Video and refresh ports.");
            if (settings.Direction == "Input" ? !current.Capture : !current.Playback)
                throw new InvalidOperationException("Direction is unavailable in the current card profile. Refresh ports.");
            device = current;
            var ndiLevel = settings.NdiReferenceLevel ? "ndi" : "unity";
            string[] args = settings.Direction == "Input"
                ? new[] { "--capture", current.Index.ToString(CultureInfo.InvariantCulture), settings.Sender, "0", settings.Quality,
                    settings.Output == "OMT + NDI" ? "omt+ndi" : settings.Output.ToLowerInvariant(), settings.Audio,
                    settings.Duplicate switch { "Add a number" => "increment", "Retry until free" => "retry", "Retry 10 minutes" => "retry10", _ => "abort" }, Key, ndiLevel }
                : new[] { "--play", current.Index.ToString(CultureInfo.InvariantCulture), settings.Source, "0", settings.Buffer.ToString(CultureInfo.InvariantCulture), settings.SourceKind.ToLowerInvariant(), settings.Audio, Key, ndiLevel };
            SettingsChanged?.Invoke();
            log.Clear(); previousLoss = 0; recentLoss = false; lastError = ""; stopping = false;
            formatLabel.Text = detailLabel.Text = "";
            var created = new EngineProcess(args);
            // Hidden tabs may not have a handle yet; they still need process callbacks.
            _ = Handle;
            engine = created;
            created.Line += line => Post(() => { if (engine == created) HandleLine(line); });
            created.Exited += code => Post(() =>
            {
                if (engine != created) return;
                engine = null; created.Dispose(); recentLoss = false;
                var reason = lastError.Length > 0 ? lastError : $"engine exited ({code})";
                if (stopping || code == 0) statusLabel.Text = "Stopped";
                else if (autoRestartBox.Checked) ScheduleRestart(reason, Environment.TickCount64 - engineStartedAt > 60000);
                else statusLabel.Text = "Error: " + reason;
                stopping = false; UpdateInputs();
            });
            engineStartedAt = Environment.TickCount64;
            created.Start();
            statusLabel.Text = automatic ? "Restarting …" : "Starting …";
        }
        catch (Exception ex)
        {
            engine?.Dispose(); engine = null;
            if (automatic && autoRestartBox.Checked) ScheduleRestart(ex.Message, false);
            else statusLabel.Text = "Cannot start: " + ex.Message;
            log.AppendText("Cannot start: " + ex.Message + Environment.NewLine);
        }
        finally { transitioning = false; UpdateInputs(); }
    }

    // A failed channel keeps its port and settings and tries again until the user stops it.
    private void ScheduleRestart(string reason, bool ranLong)
    {
        restartDelay = ranLong ? 3 : Math.Min(Math.Max(restartDelay * 2, 3), 30);
        var pending = new CancellationTokenSource();
        restartPending = pending;
        statusLabel.Text = $"Error: {reason} · restarting in {restartDelay} s";
        log.AppendText($"{DateTime.Now:HH:mm:ss} {statusLabel.Text}{Environment.NewLine}");
        _ = RestartAfterAsync(restartDelay, pending);
    }

    private async Task RestartAfterAsync(int seconds, CancellationTokenSource pending)
    {
        try { await Task.Delay(TimeSpan.FromSeconds(seconds), pending.Token); }
        catch (OperationCanceledException) { return; }
        if (IsDisposed || restartPending != pending) return;
        restartPending = null; pending.Dispose();
        await StartAsync(true);
    }

    private void Post(Action action)
    {
        try { if (!IsDisposed && IsHandleCreated) BeginInvoke(() => { if (!IsDisposed) action(); }); }
        catch (InvalidOperationException) { }
    }

    public async Task StopAsync()
    {
        if (restartPending is { } pending)
        {
            restartPending = null; pending.Cancel(); pending.Dispose();
            if (engine is null && !transitioning) { statusLabel.Text = "Stopped"; UpdateInputs(); return; }
        }
        // A start preflight must finish before Stop all / closing can complete.
        while (transitioning) await Task.Delay(25);
        var current = engine;
        if (current is null) { UpdateInputs(); return; }
        transitioning = true; stopping = true; statusLabel.Text = "Stopping …"; UpdateInputs();
        try
        {
            await current.StopAsync();
            if (engine == current) { engine = null; current.Dispose(); }
            statusLabel.Text = "Stopped"; recentLoss = false;
        }
        catch (Exception ex) { statusLabel.Text = "Stop failed: " + ex.Message; }
        finally { transitioning = false; stopping = false; UpdateInputs(); }
    }

    private async Task FindAsync()
    {
        if (Active || discovering) return;
        discovering = true; UpdateInputs();
        try
        {
            using var doc = JsonDocument.Parse(await EngineProcess.QueryAsync(kindBox.Text == "NDI" ? "--ndi-sources" : "--sources"));
            if (IsDisposed) return;
            var text = sourceBox.Text; sourceBox.Items.Clear();
            foreach (var item in doc.RootElement.EnumerateArray()) sourceBox.Items.Add(item.GetString() ?? "");
            sourceBox.Text = text;
            if (text.Length == 0 && sourceBox.Items.Count > 0) sourceBox.SelectedIndex = 0;
            statusLabel.Text = $"{sourceBox.Items.Count} sources found";
        }
        catch (Exception ex) { if (!IsDisposed) statusLabel.Text = "Search failed: " + ex.Message; }
        finally { discovering = false; if (!IsDisposed) UpdateInputs(); }
    }

    private void HandleLine(string line)
    {
        if (line.StartsWith("Health="))
        {
            try
            {
                using var doc = JsonDocument.Parse(line[7..]); var h = doc.RootElement;
                long Number(string key) => h.TryGetProperty(key, out var value) && value.TryGetInt64(out var n) ? n : 0;
                double Decimal(string key) => h.TryGetProperty(key, out var value) && value.TryGetDouble(out var n) ? n : 0;
                string mode = h.GetProperty("mode").GetString() ?? "";
                long loss;
                if (Direction == "Input")
                {
                    var signal = h.GetProperty("signal").GetBoolean() && Number("videoAgeMs") >= 0 && Number("videoAgeMs") <= 500;
                    statusLabel.Text = !signal ? "No SDI signal" : Number("connections") == 0 ? "SDI signal present · waiting for receiver" : $"Sending · {Number("connections")} receiver(s)";
                    formatLabel.Text = signal ? $"{mode} · {Number("width")} × {Number("height")} · {Decimal("frameRateN") / Math.Max(1, Decimal("frameRateD")):0.###} fps" : "";
                    loss = Number("videoLost") + Number("audioLostSamples") + Number("errors");
                    detailLabel.Text = $"Pictures: {Number("captured")}    Lost video: {Number("videoLost")}    Lost audio samples: {Number("audioLostSamples")}    Errors: {Number("errors")}\nSend/encode: avg {Decimal("videoAverageMs"):0.0} ms · P95 {Decimal("videoP95Ms"):0.0} ms · max {Decimal("videoMaximumMs"):0.0} ms";
                }
                else
                {
                    statusLabel.Text = Number("sourceAgeMs") < 0 || Number("sourceAgeMs") > 500 ? "No source video · output black" : "Playing";
                    formatLabel.Text = mode;
                    loss = Number("late") + Number("dropped");
                    detailLabel.Text = $"Pictures: {Number("receivedVideo")}    Late: {Number("late")}    Dropped: {Number("dropped")}    Repeated: {Number("repeated")}\nQueue: {Number("queueFrames")}    Card buffer: {Number("buffered")}    Resyncs: {Number("resyncs")}    Silent audio samples: {Number("silentSamples")}";
                }
                recentLoss = loss > previousLoss; previousLoss = loss;
                if (recentLoss) statusLabel.Text += " · new frame/audio errors";
            }
            catch (Exception ex) when (ex is JsonException or InvalidOperationException or KeyNotFoundException) { statusLabel.Text = "Invalid engine health report"; }
        }
        else
        {
            if (line.StartsWith("Error:")) lastError = line[6..].Trim();
            if (line.StartsWith("Notice:")) statusLabel.Text = line[7..].Trim();
            if (log.TextLength > 40000) log.Text = log.Text[^20000..];
            log.AppendText(line + Environment.NewLine);
        }
        Changed?.Invoke();
    }

    public void SampleLoad()
    {
        var sample = load.Read(engine?.Process);
        Cpu = engine is null ? "—" : sample.EngineCpu; Ram = engine is null ? "—" : sample.EngineRam;
        CpuPercent = engine is null ? 0 : load.LastEngineCpuPercent;
        RamBytes = engine is null ? 0 : load.LastEngineRamBytes;
        resourceLabel.Text = $"Channel CPU: {Cpu}    RAM: {Ram}";
    }

    public void CloseDiagnostics() { diagnostics?.Close(); log.Dispose(); }

    private void portLabel_Click(object sender, EventArgs e)
    {

    }
}
