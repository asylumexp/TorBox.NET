using Newtonsoft.Json;

namespace TorBoxNET;

public class ResponseList<T>
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

public class ResponseArray<T>
{
    [JsonProperty("success")]
    public Boolean? Success { get; set; }

    [JsonProperty("error")]
    public String? Error { get; set; }

    [JsonProperty("detail")]
    public String? Detail { get; set; }

    [JsonProperty("data")]
    public T? Data { get; set; }
}
