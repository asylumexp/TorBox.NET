using Newtonsoft.Json;

namespace TorBoxNET;

public class AvailableUsenet
{
    /// <summary>
    /// Usenet download name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Usenet download size in bytes
    /// </summary>
    [JsonProperty("size")]
    public long Size { get; set; }

    /// <summary>
    /// Usenet download hash
    /// </summary>
    [JsonProperty("hash")]
    public string Hash { get; set; } = null!;
}