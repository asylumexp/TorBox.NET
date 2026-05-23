using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TorBoxNET.Test;

public class WebDownloadsApiTest
{
    private const string TestWebDownloadUrl = "https://fuckingfast.co/o4hij09fmd3g";
    private const string TestWebDownloadName = "o4hij09fmd3g";

    [Fact]
    public async Task AddLinkAsync_SendsAuthenticatedFormAndParsesResult()
    {
        using var harness = new HttpHarness(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://api.torbox.app/v1/api/webdl/createwebdownload", request.RequestUri!.ToString());
            Assert.Equal("Bearer token-123", request.Headers.Authorization?.ToString());

            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("link=https%3A%2F%2Ffuckingfast.co%2Fo4hij09fmd3g", body);
            Assert.Contains("password=secret", body);
            Assert.Contains("name=o4hij09fmd3g", body);
            Assert.Contains("as_queued=True", body);
            Assert.Contains("add_only_if_cached=True", body);

            return JsonResponse("""{"success":true,"error":null,"detail":"started","data":{"hash":"abc","webdownload_id":42,"auth_id":"auth","jdownloader_id":null,"link_list":["https://fuckingfast.co/o4hij09fmd3g"]}}""");
        });

        var client = CreateClient(harness);

        var result = await client.WebDownloads.AddLinkAsync(TestWebDownloadUrl, "secret", TestWebDownloadName, as_queued: true, add_only_if_cached: true);

        Assert.True(result.Success);
        Assert.Equal("abc", result.Data!.Hash);
        Assert.Equal(42, result.Data.WebDownloadId);
        Assert.Single(result.Data.LinkList!);
    }

    [Fact]
    public async Task AddLinkAsync_CanUseAsyncCreateEndpoint()
    {
        using var harness = new HttpHarness(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://api.torbox.app/v1/api/webdl/asynccreatewebdownload", request.RequestUri!.ToString());
            return JsonResponse("""{"success":true,"data":{"hash":"async","webdownload_id":8}}""");
        });

        var client = CreateClient(harness);

        var result = await client.WebDownloads.AddLinkAsync(TestWebDownloadUrl, null, null, false, false, async_create: true);

        Assert.True(result.Success);
        Assert.Equal(8, result.Data!.WebDownloadId);
    }

    [Fact]
    public async Task GetCurrentAsync_ParsesList()
    {
        using var harness = new HttpHarness(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("https://api.torbox.app/v1/api/webdl/mylist?bypass_cache=True&offset=5&limit=10", request.RequestUri!.ToString());
            return JsonResponse("""{"success":true,"data":[{"id":42,"created_at":"2026-03-08T03:45:15Z","updated_at":"2026-03-08T03:45:16Z","auth_id":"auth","name":"o4hij09fmd3g","hash":"abc","download_state":"completed","download_speed":0,"original_url":"https://fuckingfast.co/o4hij09fmd3g","eta":0,"progress":100,"size":123,"download_id":"dl","files":[{"id":9,"name":"o4hij09fmd3g","size":123,"zipped":false,"infected":false}],"active":false,"cached":true,"download_present":true,"download_finished":true,"expires_at":"2026-03-09T03:45:15Z","error":null,"cached_at":"2026-03-08T04:00:00Z","server":1,"alternative_hashes":["def"],"tags":["tag"]}]}""");
        });

        var client = CreateClient(harness);

        var result = await client.WebDownloads.GetCurrentAsync(skipCache: true, offset: 5, limit: 10);

        Assert.Single(result!);
        Assert.Equal(TestWebDownloadName, result![0].Name);
        Assert.Equal(9, result[0].Files![0].Id);
        Assert.True(result[0].Cached);
    }

    [Fact]
    public async Task GetIdInfoAsync_RequestsSingleItem()
    {
        using var harness = new HttpHarness(request =>
        {
            Assert.Equal("https://api.torbox.app/v1/api/webdl/mylist?id=42&bypass_cache=True&limit=1000", request.RequestUri!.ToString());
            return JsonResponse("""{"success":true,"data":{"id":42,"hash":"abc","name":"o4hij09fmd3g"}}""");
        });

        var client = CreateClient(harness);

        var result = await client.WebDownloads.GetIdInfoAsync(42, skipCache: true);

        Assert.Equal(42, result!.Id);
        Assert.Equal("abc", result.Hash);
    }

    [Fact]
    public async Task GetIdInfoAsync_UsesCustomLimit()
    {
        using var harness = new HttpHarness(request =>
        {
            Assert.Equal("https://api.torbox.app/v1/api/webdl/mylist?id=42&bypass_cache=True&limit=10000", request.RequestUri!.ToString());
            return JsonResponse("""{"success":true,"data":{"id":42,"hash":"abc","name":"o4hij09fmd3g"}}""");
        });

        var client = CreateClient(harness);

        var result = await client.WebDownloads.GetIdInfoAsync(42, skipCache: true, limit: 10000);

        Assert.Equal(42, result!.Id);
    }

    [Fact]
    public async Task ControlByIdAsync_SendsJson()
    {
        using var harness = new HttpHarness(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://api.torbox.app/v1/api/webdl/controlwebdownload", request.RequestUri!.ToString());
            Assert.Equal("application/json", request.Content!.Headers.ContentType!.MediaType);
            var body = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"webdl_id\":42", body);
            Assert.Contains("\"operation\":\"delete\"", body);
            return JsonResponse("""{"success":true,"detail":"deleted"}""");
        });

        var client = CreateClient(harness);

        var result = await client.WebDownloads.ControlByIdAsync(42, "delete");

        Assert.True(result.Success);
    }

    [Fact]
    public async Task RequestDownloadAsync_UsesWebDownloadQueryParameters()
    {
        using var harness = new HttpHarness(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            var query = ParseQuery(request.RequestUri!);
            Assert.Equal("token-123", query["token"]);
            Assert.Equal("42", query["web_id"]);
            Assert.Equal("9", query["file_id"]);
            Assert.Equal("True", query["zip_link"]);
            Assert.Equal("1.2.3.4", query["user_ip"]);
            Assert.Equal("True", query["redirect"]);
            Assert.Equal("True", query["append_name"]);
            return JsonResponse("""{"success":true,"data":"https://cdn.torbox.app/o4hij09fmd3g"}""");
        });

        var client = CreateClient(harness);

        var result = await client.WebDownloads.RequestDownloadAsync(42, 9, zip: true, user_ip: "1.2.3.4", redirect: true, append_name: true);

        Assert.True(result.Success);
        Assert.Equal("https://cdn.torbox.app/o4hij09fmd3g", result.Data);
    }

    [Fact]
    public async Task GetAvailabilityAsync_ParsesCachedResponse()
    {
        using var harness = new HttpHarness(request =>
        {
            Assert.Equal("https://api.torbox.app/v1/api/webdl/checkcached?hash=abc&format=list&list_files=True", request.RequestUri!.ToString());
            return JsonResponse("""{"success":true,"data":[{"name":"o4hij09fmd3g","size":123,"hash":"abc","files":[{"name":"o4hij09fmd3g","size":123}]}]}""");
        });

        var client = CreateClient(harness);

        var result = await client.WebDownloads.GetAvailabilityAsync("abc", listFiles: true);

        Assert.True(result.Success);
        Assert.Equal(TestWebDownloadName, result.Data![0]!.Files![0].Name);
    }

    [Fact]
    public async Task GetHostersAsync_CanBeUnauthenticated()
    {
        using var harness = new HttpHarness(request =>
        {
            Assert.Equal("https://api.torbox.app/v1/api/webdl/hosters", request.RequestUri!.ToString());
            Assert.Null(request.Headers.Authorization);
            return JsonResponse("""{"success":true,"data":[{"id":1,"name":"Rapidgator","domains":["rapidgator.net"],"url":"https://rapidgator.net","icon":"https://fuckingfast.co/favicon.ico","status":true,"type":"hoster","note":null,"nsfw":false,"daily_link_limit":5,"daily_link_used":1,"daily_bandwidth_limit":100,"daily_bandwidth_used":10,"per_link_size_limit":50,"regex":"^https"}]}""");
        });

        var client = CreateClient(harness);

        var result = await client.WebDownloads.GetHostersAsync();

        Assert.True(result.Success);
        Assert.Equal("Rapidgator", result.Data![0].Name);
    }

    [Fact]
    public async Task EditAsync_SendsPutJson()
    {
        using var harness = new HttpHarness(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal("https://api.torbox.app/v1/api/webdl/editwebdownload", request.RequestUri!.ToString());
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"webdl_id\":42", body);
            Assert.Contains("\"name\":\"New Name\"", body);
            Assert.Contains("\"tags\":[\"video\"]", body);
            Assert.Contains("\"alternative_hashes\":[\"abc\"]", body);
            return JsonResponse("""{"success":true,"data":null}""");
        });

        var client = CreateClient(harness);

        var result = await client.WebDownloads.EditAsync(42, "New Name", new[] { "video" }, new[] { "abc" });

        Assert.True(result.Success);
    }

    [Fact]
    public async Task ErrorResponses_ThrowTorBoxException()
    {
        using var harness = new HttpHarness(_ => new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("""{"success":false,"error":"INVALID_LINK","detail":"The link is invalid."}""")
        });

        var client = CreateClient(harness);

        var exception = await Assert.ThrowsAsync<TorBoxException>(() => client.WebDownloads.AddLinkAsync("not-a-url"));

        Assert.Equal("INVALID_LINK", exception.Error);
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
