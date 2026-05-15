using Newtonsoft.Json;

namespace TorBoxNET;

public class WebDownloadHoster
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("domains")]
    public List<string>? Domains { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("icon")]
    public string? Icon { get; set; }

    [JsonProperty("status")]
    public bool Status { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("note")]
    public string? Note { get; set; }

    [JsonProperty("nsfw")]
    public bool Nsfw { get; set; }

    [JsonProperty("daily_link_limit")]
    public long DailyLinkLimit { get; set; }

    [JsonProperty("daily_link_used")]
    public long DailyLinkUsed { get; set; }

    [JsonProperty("daily_bandwidth_limit")]
    public long DailyBandwidthLimit { get; set; }

    [JsonProperty("daily_bandwidth_used")]
    public long DailyBandwidthUsed { get; set; }

    [JsonProperty("per_link_size_limit")]
    public long PerLinkSizeLimit { get; set; }

    [JsonProperty("regex")]
    public string? Regex { get; set; }
}
