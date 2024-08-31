using Newtonsoft.Json;

namespace TorBoxNET;

public class AvailableTorrent
{
    /// <summary>
    ///     Torrent name in bytes
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Torrent size in bytes
    /// </summary>
    [JsonProperty("size")]
    public long Size { get; set; }

    /// <summary>
    /// Torrent hash
    /// </summary>
    [JsonProperty("hash")]
    public string Hash { get; set; } = null!;

    /// <summary>
    /// Torrent files
    /// </summary>
    [JsonProperty("files")]
    public List<AvailableTorrentFile>? Files { get; set; } = null!;
}

public class AvailableTorrentFile
{
    /// <summary>
    /// File name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// File size
    /// </summary>
    [JsonProperty("size")]
    public long Size { get; set; }
}
