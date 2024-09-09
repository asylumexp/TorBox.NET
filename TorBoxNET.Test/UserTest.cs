using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace TorBoxNET.Test;

public class UserTest
{
    [Fact]
    public async Task User()
    {
        var client = new TorBoxNetClient();
        client.UseApiAuthentication(Setup.API_KEY);

        var result = await client.User.GetAsync(true);

        Assert.NotNull(result);
    }
}