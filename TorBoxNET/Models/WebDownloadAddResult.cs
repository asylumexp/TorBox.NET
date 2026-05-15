using Newtonsoft.Json;

namespace TorBoxNET;

public class WebDownloadAddResult
{
    [JsonProperty("hash")]
    public string? Hash { get; set; }

    [JsonProperty("webdownload_id")]
    public int? WebDownloadId { get; set; }

    [JsonProperty("auth_id")]
    public string? AuthId { get; set; }

    [JsonProperty("jdownloader_id")]
    public string? JDownloaderId { get; set; }

    [JsonProperty("link_list")]
    public List<string>? LinkList { get; set; }
}
