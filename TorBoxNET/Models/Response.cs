using Newtonsoft.Json;

namespace TorBoxNET;

internal class Response
{
    [JsonProperty("success")]
    public Boolean? Success { get; set; }

    [JsonProperty("error")]
    public String? Error { get; set; }

    [JsonProperty("detail")]
    public String? Detail { get; set; }

    [JsonProperty("data")]
    public List<dynamic>? Data { get; set; }
}