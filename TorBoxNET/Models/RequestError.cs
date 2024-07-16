using Newtonsoft.Json;

namespace TorBoxNET;

internal class RequestError
{
    [JsonProperty("error")]
    public String? Error { get; set; }

    [JsonProperty("error_code")]
    public Int32? ErrorCode { get; set; }
}