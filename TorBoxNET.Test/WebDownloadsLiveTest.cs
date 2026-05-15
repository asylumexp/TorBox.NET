using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace TorBoxNET.Test;

public class WebDownloadsLiveTest
{
    private readonly TorBoxNetClient _client;
    private readonly ITestOutputHelper _output;

    public WebDownloadsLiveTest(ITestOutputHelper output)
    {
        _output = output;
        _client = new TorBoxNetClient();
        _client.UseApiAuthentication(Setup.API_KEY);
    }

    [LiveWebDownloadFact]
    public async Task CreateWebDownload()
    {
        var webDownloadLink = Setup.WebDownloadLink;
        var webDownloadName = Setup.WebDownloadName;

        var result = await _client.WebDownloads.AddLinkAsync(
            webDownloadLink,
            name: webDownloadName,
            as_queued: false,
            add_only_if_cached: false);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);

        _output.WriteLine($"Created TorBox WebDL '{webDownloadName}'.");
        _output.WriteLine($"WebDL ID: {result.Data!.WebDownloadId}");
        _output.WriteLine($"Hash: {result.Data.Hash}");

        Assert.True(result.Data.WebDownloadId.HasValue);

        var createdItem = await WaitForWebDownloadAsync(result.Data.WebDownloadId.Value);

        Assert.NotNull(createdItem);
        Assert.Equal(result.Data.WebDownloadId.Value, createdItem!.Id);
        Assert.False(string.IsNullOrWhiteSpace(createdItem.Name));
        Assert.StartsWith(webDownloadLink, createdItem.OriginalUrl);
        Assert.True(createdItem.Cached || createdItem.DownloadPresent, $"Created WebDL is not cached or present. State: {createdItem.DownloadState}");

        _output.WriteLine($"Verified WebDL in mylist as '{createdItem.Name}' with state '{createdItem.DownloadState}'.");

        var downloadLink = await _client.WebDownloads.RequestDownloadAsync(createdItem.Id);

        Assert.True(downloadLink.Success);
        Assert.False(string.IsNullOrWhiteSpace(downloadLink.Data));

        _output.WriteLine($"Generated download link: {downloadLink.Data}");
    }

    private async Task<WebDownloadInfoResult> WaitForWebDownloadAsync(int id)
    {
        WebDownloadInfoResult lastItem = null;

        for (var attempt = 0; attempt < 10; attempt++)
        {
            var item = await _client.WebDownloads.GetIdInfoAsync(id, skipCache: true);
            if (item != null && (item.Cached || item.DownloadPresent || item.DownloadState?.Contains("failed") == true))
            {
                return item;
            }

            lastItem = item;

            await Task.Delay(2000);
        }

        return lastItem;
    }
}
