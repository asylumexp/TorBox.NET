using Newtonsoft.Json;

namespace TorBoxNET;

public class AuthenticationVerify
{
    /// <summary>
    /// The ID of the authenticated client
    /// </summary>
    [JsonProperty("client_id")]
    public String ClientId { get; set; } = null!;

    /// <summary>
    /// The secret of the client
    /// </summary>
    [JsonProperty("client_secret")]
    public String ClientSecret { get; set; } = null!;
}