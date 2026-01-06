using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace TorBoxNET;

public interface IUsenetApi
{
    /// <summary>
    /// Fetches the list of active usenet downloads for the user.
    /// </summary>
    /// <param name="skipCache">
    /// Whether to bypass the cache and retrieve fresh data from the server. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// A list of usenet downloads if the request succeeds, otherwise null.
    /// </returns>
    Task<List<UsenetInfoResult>?> GetCurrentAsync(bool skipCache = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches the list of active usenet downloads for the user.
    /// </summary>
    /// <param name="skipCache">
    /// Whether to bypass the cache and retrieve fresh data from the server. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// A list of usenet downloads if the request succeeds, otherwise null.
    /// </returns>
    Task<List<UsenetInfoResult>?> GetQueuedAsync(bool skipCache = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed information about a specific usenet download by its hash.
    /// </summary>
    /// <param name="hash">The unique hash identifier of the torrent.</param>
    /// <param name="skipCache">
    /// Whether to bypass the cache and retrieve fresh data from the server. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// Information about the download if found, otherwise null.
    /// </returns>
    Task<UsenetInfoResult?> GetHashInfoAsync(string hash, bool skipCache = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed information about a usenet download by its id.
    /// </summary>
    /// <param name="id">The unique hash identifier of the usenet download.</param>
    /// <param name="skipCache">
    /// Whether to bypass the cache and retrieve fresh data from the server. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// Information about the download if found, otherwise null.
    /// </returns>
    Task<UsenetInfoResult?> GetIdInfoAsync(int id, bool skipCache = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a nzb file to the remote client.
    /// </summary>
    /// <param name="file">The nzb file as a byte array.</param>
    /// <param name="post_processing">
    /// Proccessing preference: 
    /// -1 for default, being repair, unpack, and delete,
    /// 0 for no post proccessing actions,
    /// 1 for repair files,
    /// 2 for repair and unpack files,
    /// 3 for repair, unpack, and delete source files.
    /// </param>
    /// <param name="name">Optional name for the usenet download.</param>
    /// <param name="password">Optional password for the client to extract the files as.</param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// The response containing information about the added download.
    /// </returns>
    Task<Response<UsenetAddResult>> AddFileAsync(Byte[] file, int post_processing = -1, string? name = null, string? password = null, bool as_queued = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a nzb link to the remote client.
    /// </summary>
    /// <param name="link">The link to the publically accessible NZB file.</param>
    /// <param name="post_processing">
    /// Proccessing preference: 
    /// -1 for default, being repair, unpack, and delete,
    /// 0 for no post proccessing actions,
    /// 1 for repair files,
    /// 2 for repair and unpack files,
    /// 3 for repair, unpack, and delete source files.
    /// </param>
    /// <param name="name">Optional name for the torrent.</param>
    /// <param name="password">Optional password for the client to extract the files as.</param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// The response containing information about the added download.
    /// </returns>
    Task<Response<UsenetAddResult>> AddLinkAsync(string link, int post_processing = -1, string? name = null, string? password = null, bool as_queued = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Modifies the state of a torrent (e.g., pause, resume, reannounce, delete).
    /// </summary>
    /// <param name="hash">The unique hash of the torrent.</param>
    /// <param name="action">
    /// The action to perform: pause, resume, or delete.
    /// </param>
    /// <param name="all">Deletes all usenet downloads on account. Defaults to false.</param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// The response after performing the action.
    /// </returns>
    Task<Response> ControlAsync(string hash, string action, bool all = false,  CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the availability of a torrent (whether it's cached and ready to download).
    /// </summary>
    /// <param name="hash">The unique hash identifier of the torrent.</param>
    /// <param name="listFiles">Whether to include file list in the response.</param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// A response containing availability information for the download.
    /// </returns>
    Task<Response<List<AvailableUsenet?>>> GetAvailabilityAsync(string hash, bool listFiles = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests a download link for a specific usenet download wholly or file.
    /// </summary>
    /// <param name="usenet_id">The download id of the usenet item to download.</param>
    /// <param name="file_id">The ID of the file within the usenet item (optional).</param>
    /// <param name="zip">
    /// Whether to download the entire item as a ZIP. Defaults to false. If true, file_id is ignored.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// A response containing the download link.
    /// </returns>
    Task<Response<String>> RequestDownloadAsync(int usenet_id, int? file_id, bool zip = false, CancellationToken cancellationToken = default);
}

public class UsenetApi : IUsenetApi
{
    private readonly Requests _requests;
    private readonly Store _store;
    private readonly IQueuedApi _queued;

    internal UsenetApi(HttpClient httpClient, Store store, IQueuedApi queued)
    {
        _requests = new Requests(httpClient, store);
        _store = store;
        _queued = queued;
    }

    /// <inheritdoc />
    public async Task<List<UsenetInfoResult>?> GetCurrentAsync(bool skipCache = false, CancellationToken cancellationToken = default)
    {

        var list = await _requests.GetRequestAsync($"usenet/mylist?bypass_cache={skipCache}", true, cancellationToken);

        if (list == null)
        {
            return null;
        }

        return JsonConvert.DeserializeObject<Response<List<UsenetInfoResult>>>(list)?.Data;
    }

    /// <inheritdoc />
    public async Task<List<UsenetInfoResult>?> GetQueuedAsync(bool skipCache = false, CancellationToken cancellationToken = default)
    {

        var queuedDownloads = await _queued.GetQueuedAsync(skipCache, "usenet", null, 0, 1000, cancellationToken);

        if (queuedDownloads != null)
        {
            return queuedDownloads
                .Select(MapQueuedDownloadToUsenetInfo)
                .ToList();
        }
        return null;
    }

    /// <summary>
    /// Maps a QueuedTorrent instance to a new TorrentInfoResult instance.
    /// </summary>
    /// <param name="torrent">The QueuedTorrent to map.</param>
    /// <returns>A new TorrentInfoResult containing the mapped data.</returns>
    private UsenetInfoResult MapQueuedDownloadToUsenetInfo(QueuedDownload download)
    {
        return new UsenetInfoResult
        {
            Id = download.Id,
            Hash = download.Hash,
            Name = download.Name,
            CreatedAt = download.CreatedAt,
            DownloadState = "queued",
            Progress = 0.0,
            Files = [],
            DownloadSpeed = 0,
            UpdatedAt = download.CreatedAt
        };
    }


    /// <inheritdoc />
    public async Task<UsenetInfoResult?> GetHashInfoAsync(string hash, bool skipCache = false, CancellationToken cancellationToken = default)
    {
        var currentDownloads = await GetCurrentAsync(skipCache, cancellationToken);

        if (currentDownloads == null)
        {
            return null;
        }

        return currentDownloads.FirstOrDefault(item => item.Hash == hash);
    }

    /// <inheritdoc />
    public async Task<UsenetInfoResult?> GetIdInfoAsync(int id, bool skipCache = false, CancellationToken cancellationToken = default)
    {
        var currentDownload = await _requests.GetRequestAsync<Response<UsenetInfoResult?>>($"usenet/mylist?bypass_cache={skipCache}", true, cancellationToken);

        return currentDownload?.Data;
    }

    /// <inheritdoc />
    public async Task<Response<UsenetAddResult>> AddFileAsync(Byte[] file, int post_processing = -1, string? name = null, string? password = null, bool as_queued = false, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(file);
        fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
        {
            Name = "file",
            FileName = "nzb.nzb"
        };
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-nzb");

        content.Add(fileContent, "file");
        content.Add(new StringContent(post_processing.ToString()), "post_processing");
        content.Add(new StringContent(as_queued.ToString()), "as_queued");
        if (name != null)
        {
            content.Add(new StringContent(name), "name");
        }
        if (password != null)
        {
            content.Add(new StringContent(password), "password");
        }

        return await _requests.PostRequestMultipartAsync<Response<UsenetAddResult>>("usenet/createusenetdownload", content, true, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Response<UsenetAddResult>> AddLinkAsync(string link, int post_processing = -1, string? name = null, string? password = null, bool as_queued = false, CancellationToken cancellationToken = default)
    {
        var data = new List<KeyValuePair<string, string?>>
        {
            new KeyValuePair<string, string?>("link", link),
            new KeyValuePair<string, string?>("post_processing", post_processing.ToString()),
            new KeyValuePair<string, string?>("as_queued", as_queued.ToString()),
            //new KeyValuePair<string, string?>("name", name),
            new KeyValuePair<string, string?>("password", password),
        };

        return await _requests.PostRequestAsync<Response<UsenetAddResult>>("usenet/createusenetdownload", data, true, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Response> ControlAsync(string hash, string action, bool all = false,  CancellationToken cancellationToken = default)
    {
        var info = await GetHashInfoAsync(hash, skipCache: true, cancellationToken);

        var data = new
        {
            usenet_id = info?.Id,
            operation = action,
            all,
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        return await _requests.PostRequestRawAsync<Response>("usenet/controlusenetdownload", jsonContent, true, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Response<List<AvailableUsenet?>>> GetAvailabilityAsync(string hash, bool listFiles = false, CancellationToken cancellationToken = default)
    {
        return await _requests.GetRequestAsync<Response<List<AvailableUsenet?>>>($"usenet/checkcached?hash={hash}&format=list&list_files={listFiles}", true, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Response<String>> RequestDownloadAsync(int usenet_id, int? file_id, bool zip = false, CancellationToken cancellationToken = default)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["token"] = _store.BearerToken;
        parameters["usenet_id"] = usenet_id.ToString();
        parameters["file_id"] = file_id.ToString() ?? string.Empty;
        parameters["zip"] = zip.ToString();

        return await _requests.GetRequestAsync<Response<String>>($"usenet/requestdl?{parameters}", true, cancellationToken);

    }
}
