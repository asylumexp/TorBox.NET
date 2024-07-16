using Newtonsoft.Json;

namespace TorBoxNET;

internal class Response<T>
{
    [JsonProperty("success")]
    public Boolean? Success { get; set; }

    [JsonProperty("error")]
    public String? Error { get; set; }

    [JsonProperty("detail")]
    public String? Detail { get; set; }

    [JsonProperty("data")]
    public List<T>? Data { get; set; }
}