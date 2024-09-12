using System;
using System.Threading.Tasks;
using Xunit;

namespace TorBoxNET.Test;

public class UserTest
{
    private readonly TorBoxNetClient _client;

    public UserTest()
    {
        _client = new TorBoxNetClient();
        _client.UseApiAuthentication(Setup.API_KEY);
    }

    [Fact]
    public async Task GetUser()
    {
        var result = await _client.User.GetAsync(true);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task RefreshToken()
    {
        var sessionToken = "not_a_valid_session_token";
        var result = await _client.User.RefreshAsync(sessionToken);

        // Will always fail, but if it isn't null then the request did succeed
        Assert.NotNull(result);
    }

}
