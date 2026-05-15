using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace TorBoxNET;

public interface IWebDownloadsApi
{
    Task<List<WebDownloadInfoResult>?> GetCurrentAsync(bool skipCache = false, int offset = 0, int limit = 1000, CancellationToken cancellationToken = default);

    Task<List<WebDownloadInfoResult>?> GetQueuedAsync(bool skipCache = false, CancellationToken cancellationToken = default);

    Task<WebDownloadInfoResult?> GetIdInfoAsync(int id, bool skipCache = false, CancellationToken cancellationToken = default);

    Task<WebDownloadInfoResult?> GetHashInfoAsync(string hash, bool skipCache = false, CancellationToken cancellationToken = default);

    Task<Response<WebDownloadAddResult>> AddLinkAsync(string link, string? password = null, string? name = null, bool as_queued = false, bool add_only_if_cached = false, CancellationToken cancellationToken = default);

    Task<Response<WebDownloadAddResult>> AddLinkAsync(string link, string? password, string? name, bool as_queued, bool add_only_if_cached, bool async_create, CancellationToken cancellationToken = default);

    Task<Response> ControlAsync(string hash, string action, bool all = false, CancellationToken cancellationToken = default);

    Task<Response> ControlByIdAsync(int? webdl_id, string action, bool all = false, CancellationToken cancellationToken = default);

    Task<Response<List<AvailableWebDownload?>>> GetAvailabilityAsync(string hash, bool listFiles = false, CancellationToken cancellationToken = default);

    Task<Response<string>> RequestDownloadAsync(int web_id, int? file_id = 0, bool zip = false, string? user_ip = null, bool redirect = false, bool append_name = false, CancellationToken cancellationToken = default);

    Task<Response<List<WebDownloadHoster>?>> GetHostersAsync(bool requireAuthentication = false, CancellationToken cancellationToken = default);

    Task<Response> EditAsync(int webdl_id, string? name = null, IEnumerable<string>? tags = null, IEnumerable<string>? alternative_hashes = null, CancellationToken cancellationToken = default);
}

public class WebDownloadsApi : IWebDownloadsApi
{
    private readonly Requests _requests;
    private readonly Store _store;
    private readonly IQueuedApi _queued;

    internal WebDownloadsApi(HttpClient httpClient, Store store, IQueuedApi queued)
    {
        _requests = new Requests(httpClient, store);
        _store = store;
        _queued = queued;
    }

    public async Task<List<WebDownloadInfoResult>?> GetCurrentAsync(bool skipCache = false, int offset = 0, int limit = 1000, CancellationToken cancellationToken = default)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["bypass_cache"] = skipCache.ToString();
        parameters["offset"] = offset.ToString();
        parameters["limit"] = limit.ToString();

        var list = await _requests.GetRequestAsync($"webdl/mylist?{parameters}", true, cancellationToken);

        if (list == null)
        {
            return null;
        }

        return JsonConvert.DeserializeObject<Response<List<WebDownloadInfoResult>>>(list)?.Data;
    }

    public async Task<List<WebDownloadInfoResult>?> GetQueuedAsync(bool skipCache = false, CancellationToken cancellationToken = default)
    {
        var queuedDownloads = await _queued.GetQueuedAsync(skipCache, "webdl", null, 0, 1000, cancellationToken);

        if (queuedDownloads == null)
        {
            return null;
        }

        return queuedDownloads.Select(MapQueuedDownloadToWebDownloadInfo).ToList();
    }

    public async Task<WebDownloadInfoResult?> GetIdInfoAsync(int id, bool skipCache = false, CancellationToken cancellationToken = default)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["id"] = id.ToString();
        parameters["bypass_cache"] = skipCache.ToString();

        var webDownload = await _requests.GetRequestAsync<Response<WebDownloadInfoResult?>>($"webdl/mylist?{parameters}", true, cancellationToken);

        return webDownload?.Data;
    }

    public async Task<WebDownloadInfoResult?> GetHashInfoAsync(string hash, bool skipCache = false, CancellationToken cancellationToken = default)
    {
        var currentDownloads = await GetCurrentAsync(skipCache, cancellationToken: cancellationToken);

        if (currentDownloads != null)
        {
            var currentMatch = currentDownloads.FirstOrDefault(item => item.Hash == hash);
            if (currentMatch != null)
            {
                return currentMatch;
            }
        }

        var queuedDownloads = await GetQueuedAsync(skipCache, cancellationToken);
        return queuedDownloads?.FirstOrDefault(item => item.Hash == hash);
    }

    public async Task<Response<WebDownloadAddResult>> AddLinkAsync(string link, string? password = null, string? name = null, bool as_queued = false, bool add_only_if_cached = false, CancellationToken cancellationToken = default)
    {
        return await AddLinkAsync(link, password, name, as_queued, add_only_if_cached, false, cancellationToken);
    }

    public async Task<Response<WebDownloadAddResult>> AddLinkAsync(string link, string? password, string? name, bool as_queued, bool add_only_if_cached, bool async_create, CancellationToken cancellationToken = default)
    {
        var data = new List<KeyValuePair<string, string?>>
        {
            new KeyValuePair<string, string?>("link", link),
            new KeyValuePair<string, string?>("password", password),
            new KeyValuePair<string, string?>("name", name),
            new KeyValuePair<string, string?>("as_queued", as_queued.ToString()),
            new KeyValuePair<string, string?>("add_only_if_cached", add_only_if_cached.ToString())
        };

        var endpoint = async_create ? "webdl/asynccreatewebdownload" : "webdl/createwebdownload";

        return await _requests.PostRequestAsync<Response<WebDownloadAddResult>>(endpoint, data, true, cancellationToken);
    }

    public async Task<Response> ControlAsync(string hash, string action, bool all = false, CancellationToken cancellationToken = default)
    {
        var info = await GetHashInfoAsync(hash, skipCache: true, cancellationToken);
        return await ControlByIdAsync(info?.Id, action, all, cancellationToken);
    }

    public async Task<Response> ControlByIdAsync(int? webdl_id, string action, bool all = false, CancellationToken cancellationToken = default)
    {
        var data = new
        {
            webdl_id,
            operation = action,
            all
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        return await _requests.PostRequestRawAsync<Response>("webdl/controlwebdownload", jsonContent, true, cancellationToken);
    }

    public async Task<Response<List<AvailableWebDownload?>>> GetAvailabilityAsync(string hash, bool listFiles = false, CancellationToken cancellationToken = default)
    {
        return await _requests.GetRequestAsync<Response<List<AvailableWebDownload?>>>($"webdl/checkcached?hash={hash}&format=list&list_files={listFiles}", true, cancellationToken);
    }

    public async Task<Response<string>> RequestDownloadAsync(int web_id, int? file_id = 0, bool zip = false, string? user_ip = null, bool redirect = false, bool append_name = false, CancellationToken cancellationToken = default)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["token"] = _store.BearerToken;
        parameters["web_id"] = web_id.ToString();
        parameters["file_id"] = file_id.ToString();
        parameters["zip_link"] = zip.ToString();
        parameters["redirect"] = redirect.ToString();
        parameters["append_name"] = append_name.ToString();
        if (!string.IsNullOrWhiteSpace(user_ip))
        {
            parameters["user_ip"] = user_ip;
        }

        return await _requests.GetRequestAsync<Response<string>>($"webdl/requestdl?{parameters}", true, cancellationToken);
    }

    public async Task<Response<List<WebDownloadHoster>?>> GetHostersAsync(bool requireAuthentication = false, CancellationToken cancellationToken = default)
    {
        return await _requests.GetRequestAsync<Response<List<WebDownloadHoster>?>>("webdl/hosters", requireAuthentication, cancellationToken);
    }

    public async Task<Response> EditAsync(int webdl_id, string? name = null, IEnumerable<string>? tags = null, IEnumerable<string>? alternative_hashes = null, CancellationToken cancellationToken = default)
    {
        var data = new
        {
            webdl_id,
            name,
            tags,
            alternative_hashes
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        return await _requests.PutRequestRawAsync<Response>("webdl/editwebdownload", jsonContent, true, cancellationToken);
    }

    private static WebDownloadInfoResult MapQueuedDownloadToWebDownloadInfo(QueuedDownload download)
    {
        return new WebDownloadInfoResult
        {
            Id = download.Id,
            Hash = download.Hash,
            Name = download.Name,
            CreatedAt = download.CreatedAt,
            UpdatedAt = download.CreatedAt,
            DownloadState = "queued",
            Progress = 0.0,
            Files = [],
            DownloadSpeed = 0
        };
    }
}
