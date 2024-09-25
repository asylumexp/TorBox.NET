using Newtonsoft.Json;

namespace TorBoxNET;

public class UsenetAddResult
{
    [JsonProperty("hash")]
    public string? Hash { get; set; }

    [JsonProperty("usenetdownload_id")]
    public int? UsenetDownloadId { get; set; }

    [JsonProperty("auth_id")]
    public string? AuthId { get; set; }
}
