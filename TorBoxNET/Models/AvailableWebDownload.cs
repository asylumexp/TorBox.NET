using Newtonsoft.Json;

namespace TorBoxNET;

public class AvailableWebDownload
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("size")]
    public long Size { get; set; }

    [JsonProperty("hash")]
    public string? Hash { get; set; }

    [JsonProperty("files")]
    public List<AvailableWebDownloadFile>? Files { get; set; }
}

public class AvailableWebDownloadFile
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("size")]
    public long Size { get; set; }
}
