using Newtonsoft.Json;

namespace TorBoxNET;

public class TorrentAddResult
{
    [JsonProperty("hash")]
    public string? Hash { get; set; }

    [JsonProperty("torrent_id")]
    public int? TorrentID { get; set; }

    [JsonProperty("auth_id")]
    public string? AuthID { get; set; }
}
