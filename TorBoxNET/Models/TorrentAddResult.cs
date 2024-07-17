using Newtonsoft.Json;

namespace TorBoxNET;

public class TorrentAddResult
{
    public string? Hash { get; set; }
    public int? Torrent_ID { get; set; }
    public string? Auth_ID { get; set; }
}
