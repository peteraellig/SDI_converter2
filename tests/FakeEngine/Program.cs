using System.Text.Json;

// Hardware-free process fixture. Never distributed with the application.
var scenario = Environment.GetEnvironmentVariable("SDI_TEST_CASE") ?? "";
if (args[0] == "--list")
{
    if (scenario == "slow") await Task.Delay(400);
    var devices = Enumerable.Range(0, 4).Select(i => new
    {
        index = scenario == "reorder" ? 3 - i : i, key = "fixture-" + i, name = $"Simulated SDI ({i + 1})",
        capture = true, playback = true, busy = scenario == "busy" ? 1 : 0
    });
    Console.WriteLine(JsonSerializer.Serialize(scenario == "removed" ? devices.Take(0) : devices)); return;
}
if (args[0] is "--sources" or "--ndi-sources") { Console.WriteLine("[\"TEST (source)\"]"); return; }
if (args[2] == "crash") { Console.Error.WriteLine("Synthetic startup failure"); Environment.ExitCode = 7; return; }
var directory = Path.Combine(AppContext.BaseDirectory, "evidence"); Directory.CreateDirectory(directory);
File.WriteAllText(Path.Combine(directory, $"args-{args[1]}.json"), JsonSerializer.Serialize(args));
var stop = Task.Run(() => Console.ReadLine());
int frames = 0;
while (!stop.IsCompleted)
{
    await Task.WhenAny(stop, Task.Delay(70));
    Console.WriteLine("Health=" + JsonSerializer.Serialize(new
    {
        mode = "1080p50", signal = true, connections = 1, videoAgeMs = 0, width = 1920, height = 1080,
        frameRateN = 50, frameRateD = 1, captured = ++frames, videoLost = 0, audioLostSamples = 0, errors = 0,
        videoAverageMs = 1.0, videoP95Ms = 1.2, videoMaximumMs = 2.0, sourceAgeMs = 0,
        receivedVideo = frames, late = 0, dropped = 0, repeated = 0, queueFrames = 3, buffered = 3, resyncs = 0, silentSamples = 0
    }));
}

