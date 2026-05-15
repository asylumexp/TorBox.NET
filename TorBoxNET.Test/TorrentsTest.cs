using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace TorBoxNET.Test;

public class TorrentsTest
{
    private readonly TorBoxNetClient _client;
    private readonly ITestOutputHelper _output;

    public TorrentsTest(ITestOutputHelper output)
    {
        _output = output;
        _client = new TorBoxNetClient();
        _client.UseApiAuthentication(Setup.API_KEY);
    }

    [LiveFact]
    public async Task TorrentsCount()
    {
        var result = await _client.Torrents.GetTotal();

        _output.WriteLine(result.ToString());

        Assert.NotEqual(-1, result);
    }

    [LiveFact]
    public async Task CurrentTorrents()
    {
        var result = await _client.Torrents.GetCurrentAsync(true);

        foreach (var torrent in result)
        {
            _output.WriteLine(torrent.Name);
        }

        Assert.NotNull(result);
    }

    [LiveFact]
    public async Task QueuedTorrents()
    {
        var result = await _client.Torrents.GetQueuedAsync(true);

        var jsonOutput = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });

        _output.WriteLine(jsonOutput);

        Assert.NotNull(result);
    }

    [LiveFact]
    public async Task Info()
    {
        var result = await _client.Torrents.GetHashInfoAsync("dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c");

        if (result == null)
        {
            _output.WriteLine("Torrent dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c was not found on this account.");
            return;
        }

        _output.WriteLine(result.Name);

        Assert.NotNull(result);
    }

    [LiveFact]
    public async Task AddFile()
    {
        const String filePath = @"big-buck-bunny.torrent";

        var fileBytes = await File.ReadAllBytesAsync(filePath);

        var result = await _client.Torrents.AddFileAsync(fileBytes, 1, false, "Big Buck Bunny");

        Assert.True(result.Success);
    }

    [LiveFact]
    public async Task AddMagnet()
    {
        var magnetLink = "magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c&dn=Big+Buck+Bunny&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&tr=wss%3A%2F%2Ftracker.fastcast.nz&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F";
        var result = await _client.Torrents.AddMagnetAsync(magnetLink);

        Assert.True(result.Success);
    }

    [LiveFact]
    public async Task ControlTorrent()
    {
        var hash = "dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c";
        var action = "delete";

        var result = await _client.Torrents.ControlAsync(hash, action);

        var magnetLink = "magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c&dn=Big+Buck+Bunny&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&tr=wss%3A%2F%2Ftracker.fastcast.nz&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F";
        await _client.Torrents.AddMagnetAsync(magnetLink);

        Assert.True(result.Success);
    }

    [LiveFact]
    public async Task CheckAvailability()
    {
        var hash = "dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c";
        var result = await _client.Torrents.GetAvailabilityAsync(hash, false);

        Assert.True(result.Success);
    }

    [LiveFact]
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
