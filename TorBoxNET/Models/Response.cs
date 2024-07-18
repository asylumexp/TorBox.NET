using Newtonsoft.Json;

namespace TorBoxNET
{
    public class Response
    {
        [JsonProperty("success")]
        public Boolean? Success { get; set; }

        [JsonProperty("error")]
        public String? Error { get; set; }

        [JsonProperty("detail")]
        public String? Detail { get; set; }
    }
    public class Response<T> : Response
    {
        [JsonProperty("data")]
        public T? Data { get; set; }
    }
}
