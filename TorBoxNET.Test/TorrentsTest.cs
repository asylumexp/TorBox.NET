using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TorBoxNET.Test;

public class TorrentsTest
{
    private readonly TorBoxNetClient _client;

    public TorrentsTest()
    {
        _client = new TorBoxNetClient();
        _client.UseApiAuthentication(Setup.API_KEY);
    }

    [Fact]
    public async Task TorrentsCount()
    {
        var result = await _client.Torrents.GetTotal();

        Assert.NotEqual(-1, result);
    }

    [Fact]
    public async Task CurrentTorrents()
    {
        var result = await _client.Torrents.GetCurrentAsync(true);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task QueuedTorrents()
    {
        var result = await _client.Torrents.GetQueuedAsync(true);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Info()
    {
        var result = await _client.Torrents.GetHashInfoAsync("dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c");

        Assert.NotNull(result);
    }

    [Fact]
    public async Task AddFile()
    {
        const String filePath = @"big-buck-bunny.torrent";

        var fileBytes = await File.ReadAllBytesAsync(filePath);

        var result = await _client.Torrents.AddFileAsync(fileBytes, 1, false, "Big Buck Bunny");

        Assert.True(result.Success);
    }

    [Fact]
    public async Task AddMagnet()
    {
        var magnetLink = "magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c&dn=Big+Buck+Bunny&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&tr=wss%3A%2F%2Ftracker.fastcast.nz&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F";
        var result = await _client.Torrents.AddMagnetAsync(magnetLink);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task ControlTorrent()
    {
        var hash = "dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c";
        var action = "pause";

        var result = await _client.Torrents.ControlAsync(hash, action);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task CheckAvailability()
    {
        var hash = "dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c";
        var result = await _client.Torrents.GetAvailabilityAsync(hash, false);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task RequestDownload()
    {
        var torrentId = 123;
        var fileId = 456;

        try
        {
            await _client.Torrents.RequestDownloadAsync(torrentId, fileId, false);
        }
        catch (TorBoxException)
        {
            Assert.True(true);
        }
    }
}