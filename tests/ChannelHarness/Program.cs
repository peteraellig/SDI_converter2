using SdiConverter;
using System.Diagnostics;
using System.Text.Json;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        var config = new ChannelConfiguration { StoragePath = Path.Combine(AppContext.BaseDirectory, "fixture-settings.json") };
        using var form = new MultiChannelForm(config);
        form.Text = "SDI Converter 2 · SIMULATED PORTS · TEST";
        if (args.Contains("--test")) form.Shown += async (_, _) =>
        {
            var channels = new List<ChannelControl>();
            try
            {
                var watch = Stopwatch.StartNew();
                while (form.Controls.OfType<TabControl>().Single().TabPages.Count != 4 && watch.ElapsedMilliseconds < 5000) await Task.Delay(30);
                channels = form.Controls.OfType<TabControl>().Single().TabPages.Cast<TabPage>().Select(p => p.Controls.OfType<ChannelControl>().Single()).ToList();
                Check(channels.Count == 4, "Four detected ports create four channels");
                Check(channels.Count(c => c.Direction == "Input") == 3 && channels[3].Direction == "Output", "Default routing is three inputs / one output");
                channels[3].Controls.Find("sourceBox", true).Single().Text = "TEST (source)";
                await Task.WhenAll(channels.Select(c => c.StartAsync()));
                await Task.Delay(450);
                Check(channels.All(c => c.Active), "Four independent engine processes run simultaneously");
                Check(channels.Take(3).All(c => c.Status.StartsWith("Sending")) && channels[3].Status == "Playing", "Health reports reach the correct channels");
                await Task.Delay(200); foreach (var channel in channels) channel.SampleLoad();
                Check(channels.All(c => c.RamBytes > 0), "Per-channel memory measurement covers all engines");
                await channels[1].StopAsync();
                Check(!channels[1].Active && channels.Where((c, i) => i != 1).All(c => c.Active), "Stopping one channel leaves the other three running");
                await channels[1].StartAsync(); await Task.Delay(200);
                Check(channels[1].Active, "Stopped channel restarts");
                await Task.WhenAll(channels.Select(c => c.StopAsync()));
                Check(channels.All(c => !c.Active), "Stop all releases every process");
                Environment.SetEnvironmentVariable("SDI_TEST_CASE", "busy");
                await channels[0].StartAsync(); Check(!channels[0].Active && channels[0].Status.Contains("busy"), "Busy port is rejected");
                Environment.SetEnvironmentVariable("SDI_TEST_CASE", "removed");
                await channels[0].StartAsync(); Check(!channels[0].Active && channels[0].Status.Contains("disconnected"), "Disconnected port is rejected");
                Environment.SetEnvironmentVariable("SDI_TEST_CASE", "reorder");
                await channels[0].StartAsync(); await Task.Delay(150);
                var command = JsonSerializer.Deserialize<string[]>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "evidence", "args-3.json")))!;
                Check(command[1] == "3" && command[^1] == "fixture-0", "Reordered enumeration is resolved through persistent identity");
                await channels[0].StopAsync();
                Environment.SetEnvironmentVariable("SDI_TEST_CASE", "slow");
                var start = channels[0].StartAsync(); var stop = channels[0].StopAsync(); await Task.WhenAll(start, stop);
                Check(!channels[0].Active, "Stop during preflight waits and cleans up the newly started engine");
                Environment.SetEnvironmentVariable("SDI_TEST_CASE", null);
                channels[0].Controls.Find("senderBox", true).Single().Text = "crash";
                await channels[0].StartAsync(); await Task.Delay(300);
                Check(!channels[0].Active && channels[0].Status.Contains("Synthetic startup failure"), "Immediate engine failure returns the channel to idle");
                channels[0].Controls.Find("senderBox", true).Single().Text = "";
                await channels[0].StartAsync(); Check(!channels[0].Active && channels[0].Status.Contains("sender name"), "Missing configuration does not launch an engine");
                var saved = JsonSerializer.Deserialize<ChannelConfiguration>(File.ReadAllText(config.StoragePath))!;
                Check(saved.Channels.Count == 4 && saved.Channels["fixture-3"].Source == "TEST (source)", "Independent channel settings persist");
                Console.WriteLine("PASS: all channel integration checks");
            }
            catch (Exception ex) { Console.Error.WriteLine(ex); Environment.ExitCode = 1; }
            finally
            {
                Environment.SetEnvironmentVariable("SDI_TEST_CASE", null);
                await Task.WhenAll(channels.Select(c => c.StopAsync()));
                form.Dispose(); Application.ExitThread();
            }
        };
        Application.Run(form);
    }
    static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception("FAIL: " + message);
        Console.WriteLine("PASS: " + message);
    }
}
