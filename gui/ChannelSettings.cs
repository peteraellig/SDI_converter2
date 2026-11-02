using System.Text.Json;

namespace SdiConverter;

public sealed record SdiDevice(int Index, string Key, string Name, bool Capture, bool Playback, long? Busy)
{
    public static async Task<List<SdiDevice>> DiscoverAsync()
    {
        using var doc = JsonDocument.Parse(await EngineProcess.QueryAsync("--list"));
        return doc.RootElement.EnumerateArray().Select(d => new SdiDevice(
            d.GetProperty("index").GetInt32(), d.GetProperty("key").GetString()!, d.GetProperty("name").GetString()!,
            d.GetProperty("capture").GetBoolean(), d.GetProperty("playback").GetBoolean(),
            d.GetProperty("busy").ValueKind == JsonValueKind.Number ? d.GetProperty("busy").GetInt64() : null)).ToList();
    }
}

public sealed class ChannelSettings
{
    public string Direction { get; set; } = "Input";
    public string Sender { get; set; } = "";
    public string Output { get; set; } = "OMT";
    public string SourceKind { get; set; } = "OMT";
    public string Source { get; set; } = "";
    public string Quality { get; set; } = "Normal";
    public string Audio { get; set; } = "Stereo 1&2";
    public string Duplicate { get; set; } = "Abort";
    public int Buffer { get; set; } = 3;
}

public sealed class ChannelConfiguration
{
    public Dictionary<string, ChannelSettings> Channels { get; set; } = new();
    public static string FilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDI Converter 2", "channels.json");
    [System.Text.Json.Serialization.JsonIgnore]
    public string StoragePath { get; set; } = FilePath;
    public static ChannelConfiguration Load()
    {
        try
        {
            var value = JsonSerializer.Deserialize<ChannelConfiguration>(File.ReadAllText(FilePath)) ?? new();
            value.Channels ??= new();
            return value;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException) { return new(); }
    }
    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(StoragePath)!);
        File.WriteAllText(StoragePath + ".tmp", JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(StoragePath + ".tmp", StoragePath, true);
    }
}
