using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace TorBoxNET.Test;

public class TorrentsTest
{
    [Fact]
    public async Task TorrentsCount()
    {
        var client = new TorBoxNetClient();
        client.UseApiAuthentication(Setup.API_KEY);

        var result = await client.Torrents.GetTotal();

        Assert.DoesNotMatch("0", result.ToString());
    }

    [Fact]
    public async Task CurrentTorrents()
    {
        var client = new TorBoxNetClient();
        client.UseApiAuthentication(Setup.API_KEY);

        var result = await client.Torrents.GetCurrentAsync(true);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task QueuedTorrents()
    {
        var client = new TorBoxNetClient();
        client.UseApiAuthentication(Setup.API_KEY);

        var result = await client.Torrents.GetQueuedAsync(true);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Info()
    {
        var client = new TorBoxNetClient();
        client.UseApiAuthentication(Setup.API_KEY);

        var result = await client.Torrents.GetInfoAsync("dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c");

        Assert.Equal("dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c", result.Hash);
    }
}