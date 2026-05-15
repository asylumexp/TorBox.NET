using Newtonsoft.Json;

namespace TorBoxNET;

public class TorrentAddResult
{
    [JsonProperty("hash")]
    public string? Hash { get; set; }

    [JsonProperty("torrent_id")]
    public int? TorrentId { get; set; }

    [JsonProperty("auth_id")]
    public string? AuthId { get; set; }
}
