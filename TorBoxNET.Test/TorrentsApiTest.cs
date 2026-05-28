using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TorBoxNET.Test;

public class TorrentsApiTest
{
    [Fact]
    public async Task AddMagnetAsync_SendsMultipartForm()
    {
        using var harness = new HttpHarness(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://api.torbox.app/v1/api/torrents/createtorrent", request.RequestUri!.ToString());
            Assert.Equal("multipart/form-data", request.Content!.Headers.ContentType!.MediaType);

            var body = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("name=magnet", body);
            Assert.Contains("magnet:?xt=urn:btih:abc", body);
            Assert.Contains("name=seed", body);
            Assert.Contains("name=allow_zip", body);
            Assert.Contains("name=as_queued", body);
            Assert.Contains("name=add_only_if_cached", body);

            return JsonResponse("""{"success":true,"data":{"hash":"abc","torrent_id":42}}""");
        });

        var client = CreateClient(harness);

        var result = await client.Torrents.AddMagnetAsync("magnet:?xt=urn:btih:abc", 3, false, null, true, true, false);

        Assert.True(result.Success);
        Assert.Equal("abc", result.Data!.Hash);
        Assert.Equal(42, result.Data.TorrentId);
    }

    [Fact]
    public async Task AddMagnetAsync_CanUseAsyncCreateEndpoint()
    {
        using var harness = new HttpHarness(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://api.torbox.app/v1/api/torrents/asynccreatetorrent", request.RequestUri!.ToString());
            return JsonResponse("""{"success":true,"data":{"hash":"async","torrent_id":8}}""");
        });

        var client = CreateClient(harness);

        var result = await client.Torrents.AddMagnetAsync("magnet:?xt=urn:btih:abc", 3, false, null, false, false, true);

        Assert.True(result.Success);
        Assert.Equal(8, result.Data!.TorrentId);
    }

    [Fact]
    public async Task ControlAsync_SendsQueuedIdForQueuedTorrent()
    {
        var requestIndex = 0;
        using var harness = new HttpHarness(request =>
        {
            requestIndex++;

            if (requestIndex == 1)
            {
                Assert.Equal("https://api.torbox.app/v1/api/torrents/mylist?bypass_cache=true&limit=1000", request.RequestUri!.ToString());
                return JsonResponse("""{"success":true,"data":[]}""");
            }

            if (requestIndex == 2)
            {
                Assert.Equal("https://api.torbox.app/v1/api/queued/getqueued?type=torrent&bypass_cache=true&offset=0&limit=1000", request.RequestUri!.ToString());
                return JsonResponse("""{"success":true,"data":[{"id":99,"hash":"abc","name":"queued","magnet":"magnet:?xt=urn:btih:abc","created_at":"2026-03-08T03:45:15Z","torrent_file":false}]}""");
            }

            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://api.torbox.app/v1/api/torrents/controlqueued", request.RequestUri!.ToString());
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"queued_id\":99", body);
            Assert.Contains("\"operation\":\"delete\"", body);
            return JsonResponse("""{"success":true}""");
        });

        var client = CreateClient(harness);

        var result = await client.Torrents.ControlAsync("abc", "delete");

        Assert.True(result.Success);
        Assert.Equal(3, requestIndex);
    }

    [Fact]
    public async Task RequestDownloadAsync_UsesTorrentQueryParameters()
    {
        using var harness = new HttpHarness(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            var query = ParseQuery(request.RequestUri!);
            Assert.Equal("token-123", query["token"]);
            Assert.Equal("42", query["torrent_id"]);
            Assert.Equal("9", query["file_id"]);
            Assert.Equal("true", query["zip_link"]);
            Assert.Equal("1.2.3.4", query["user_ip"]);
            Assert.Equal("true", query["redirect"]);
            Assert.Equal("true", query["append_name"]);
            return JsonResponse("""{"success":true,"data":"https://cdn.torbox.app/file"}""");
        });

        var client = CreateClient(harness);

        var result = await client.Torrents.RequestDownloadAsync(42, 9, zip: true, user_ip: "1.2.3.4", redirect: true, append_name: true);

        Assert.True(result.Success);
        Assert.Equal("https://cdn.torbox.app/file", result.Data);
    }

    private static TorBoxNetClient CreateClient(HttpHarness harness)
    {
        var client = new TorBoxNetClient(httpClient: harness.Client, retryCount: 0);
        client.UseApiAuthentication("token-123");
        return client;
    }

    private static HttpResponseMessage JsonResponse(string json)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
    }

    private static Dictionary<string, string> ParseQuery(Uri uri)
    {
        return uri.Query.TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Split('=', 2))
            .ToDictionary(parts => Uri.UnescapeDataString(parts[0]), parts => Uri.UnescapeDataString(parts[1]));
    }

    private sealed class HttpHarness : IDisposable
    {
        public HttpHarness(Func<HttpRequestMessage, HttpResponseMessage> respond)
        {
            Client = new HttpClient(new Handler(respond));
        }

        public HttpClient Client { get; }

        public void Dispose()
        {
            Client.Dispose();
        }

        private sealed class Handler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

            public Handler(Func<HttpRequestMessage, HttpResponseMessage> respond)
            {
                _respond = respond;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_respond(request));
            }
        }
    }
}
