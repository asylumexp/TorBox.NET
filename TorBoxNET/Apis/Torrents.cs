using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace TorBoxNET;

/// <summary>
/// Provides methods for interacting with the torrent-related API endpoints, including adding, 
/// retrieving, and controlling torrents, as well as requesting download links.
/// </summary>
public interface ITorrentsApi
{
    /// <summary>
    /// Retrieves the total number of user torrents.
    /// </summary>
    /// <param name="skipCache">
    /// Whether to bypass the cache and fetch fresh data from the server. Defaults to false.
    /// </param>
    /// <param name="limit">
    /// Maximum number of torrents to request from the API. Defaults to 1000.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// The total number of torrents, or -1 if the request fails.
    /// </returns>
    Task<Int64> GetTotal(bool skipCache = false, int limit = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches the list of active torrents for the user.
    /// </summary>
    /// <param name="skipCache">
    /// Whether to bypass the cache and retrieve fresh data from the server. Defaults to false.
    /// </param>
    /// <param name="limit">
    /// Maximum number of torrents to request from the API. Defaults to 1000.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// A list of torrents if the request succeeds, otherwise null.
    /// </returns>
    Task<List<TorrentInfoResult>?> GetCurrentAsync(bool skipCache = false, int limit = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of user's queued torrents.
    /// </summary>
    /// <param name="skipCache">Whether to bypass the cache and retrieve fresh data from the server. Defaults to false.</param>
    /// <param name="cancellationToken">A token to cancel the task if necessary.</param>
    /// <returns>A list of TorrentInfoResult, an empty list if nothing is found, null if request failed.</returns>
    Task<List<TorrentInfoResult>?> GetQueuedAsync(
        bool skipCache = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed information about a specific torrent by its ID.
    /// Checks both active and queued torrents.
    /// </summary>
    /// <param name="id">The unique identifier of the torrent.</param>
    /// <param name="skipCache">
    /// Whether to bypass the cache and retrieve fresh data from the server. Defaults to false.
    /// </param>
    /// <param name="limit">
    /// Maximum number of torrents to request from the API. Defaults to 1000.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// Information about the torrent if found, otherwise null.
    /// </returns>
    Task<TorrentInfoResult?> GetIdInfoAsync(int id, bool skipCache = false, int limit = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed information about a specific torrent by its hash.
    /// Checks both active and queued torrents.
    /// </summary>
    /// <param name="hash">The unique hash identifier of the torrent.</param>
    /// <param name="skipCache">
    /// Whether to bypass the cache and retrieve fresh data from the server. Defaults to false.
    /// </param>
    /// <param name="limit">
    /// Maximum number of torrents to request from the API. Defaults to 1000.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// Information about the torrent if found, otherwise null.
    /// </returns>
    /// <remarks>
    /// Deprecated: On or after 31 August 2026, GetHashInfoAsync will be removed.
    /// Migrate to GetIdInfoAsync and use torrent ID instead of torrent hash.
    /// </remarks>
    [Obsolete("Torrents.GetHashInfoAsync will be removed on or after 31 August 2026. Migrate to Torrents.GetIdInfoAsync and use torrent ID instead of torrent hash.")]
    Task<TorrentInfoResult?> GetHashInfoAsync(string hash, bool skipCache = false, int limit = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a torrent file to the torrent client.
    /// </summary>
    /// <param name="file">The torrent file as a byte array.</param>
    /// <param name="seeding">
    /// Seeding preference: 1 for auto, 2 for seed, and 3 for no seed.
    /// </param>
    /// <param name="allowZip">Whether to allow zipped torrents.</param>
    /// <param name="name">Optional name for the torrent.</param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// The response containing information about the added torrent.
    /// </returns>
    Task<Response<TorrentAddResult>> AddFileAsync(Byte[] file, int seeding = 1, bool allowZip = false, string? name = null, bool as_queued = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a torrent file to the torrent client.
    /// </summary>
    Task<Response<TorrentAddResult>> AddFileAsync(Byte[] file, int seeding, bool allowZip, string? name, bool as_queued, bool add_only_if_cached, bool async_create, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a magnet link to the torrent client.
    /// </summary>
    /// <param name="magnet">The magnet link to be added.</param>
    /// <param name="seeding">
    /// Seeding preference: 1 for auto, 2 for seed, and 3 for no seed.
    /// </param>
    /// <param name="allowZip">Whether to allow zipped torrents.</param>
    /// <param name="name">Optional name for the torrent.</param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// The response containing information about the added torrent.
    /// </returns>
    Task<Response<TorrentAddResult>> AddMagnetAsync(string magnet, int seeding = 1, bool allowZip = false, string? name = null, bool as_queued = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a magnet link to the torrent client.
    /// </summary>
    Task<Response<TorrentAddResult>> AddMagnetAsync(string magnet, int seeding, bool allowZip, string? name, bool as_queued, bool add_only_if_cached, bool async_create, CancellationToken cancellationToken = default);

    /// <summary>
    /// Modifies the state of a torrent (e.g., pause, resume, reannounce, delete).
    /// </summary>
    /// <param name="hash">The unique hash of the torrent.</param>
    /// <param name="action">
    /// The action to perform: pause, resume, reannounce, or delete.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// The response after performing the action.
    /// </returns>
    /// <remarks>
    /// Deprecated: On or after 31 August 2026, ControlAsync will require a torrentId
    /// instead of a hash. Migrate to ControlByIdAsync.
    /// </remarks>
    [Obsolete("Torrents.ControlAsync currently accepts a torrent hash, but on or after 31 August 2026 it will require a torrentId instead. Migrate to Torrents.ControlByIdAsync.")]
    Task<Response> ControlAsync(string hash, string action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Modifies the state of a torrent by TorBox torrent ID.
    /// </summary>
    /// <remarks>
    /// On or after 31 August 2026, ControlByIdAsync is expected to become an alias for
    /// ControlAsync after ControlAsync changes to require a torrentId instead of a hash.

    /// </remarks>
    Task<Response> ControlByIdAsync(int torrentId, string action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the availability of a torrent (whether it's cached and ready to download).
    /// </summary>
    /// <param name="hash">The unique hash identifier of the torrent.</param>
    /// <param name="listFiles">Whether to include file list in the response.</param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// A response containing availability information for the torrent.
    /// </returns>
    Task<Response<List<AvailableTorrent?>>> GetAvailabilityAsync(string hash, bool listFiles = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests a download link for a specific torrent or file.
    /// </summary>
    /// <param name="torrent_id">The ID of the torrent to download.</param>
    /// <param name="file_id">The ID of the file within the torrent (optional).</param>
    /// <param name="zip">
    /// Whether to download the entire torrent as a ZIP. If true, file_id is ignored.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// A response containing the download link.
    /// </returns>
    Task<Response<string>> RequestDownloadAsync(int torrent_id, int? file_id, bool zip = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests a download link for a specific torrent or file.
    /// </summary>
    Task<Response<string>> RequestDownloadAsync(int torrent_id, int? file_id, bool zip, string? user_ip, bool redirect, bool append_name, CancellationToken cancellationToken = default);
}

/// <inheritdoc />
public class TorrentsApi : ITorrentsApi
{
    private readonly Requests _requests;
    private readonly Store _store;
    private readonly IQueuedApi _queued;

    internal TorrentsApi(HttpClient httpClient, Store store, IQueuedApi queued)
    {
        _requests = new Requests(httpClient, store);
        _store = store;
        _queued = queued;
    }

    /// <inheritdoc />
    public async Task<Int64> GetTotal(bool skipCache = false, int limit = 1000, CancellationToken cancellationToken = default)
    {
        var res = await GetCurrentAsync(skipCache, limit, cancellationToken);

        if (res == null)
        {
            return -1;
        }

        return res.Count;
    }

    /// <inheritdoc />
    public async Task<List<TorrentInfoResult>?> GetCurrentAsync(bool skipCache = false, int limit = 1000, CancellationToken cancellationToken = default)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["bypass_cache"] = skipCache.ToString().ToLowerInvariant();
        parameters["limit"] = limit.ToString();

        var list = await _requests.GetRequestAsync($"torrents/mylist?{parameters}", true, cancellationToken);

        if (list == null)
        {
            return null;
        }

        var response = JsonConvert.DeserializeObject<Response<List<TorrentInfoResult>>>(list);
        return response?.Data;
    }

    /// <inheritdoc />
    public async Task<List<TorrentInfoResult>?> GetQueuedAsync(
        bool skipCache = false,
        CancellationToken cancellationToken = default)
    {
        var queuedTorrents = await _queued.GetQueuedAsync(skipCache, "torrent", null, 0, 1000, cancellationToken);

        if (queuedTorrents != null)
        {
            return queuedTorrents
                .Select(MapQueuedTorrentToTorrentInfo)
                .ToList();
        }
        return null;
    }

    /// <summary>
    /// Maps a QueuedTorrent instance to a new TorrentInfoResult instance.
    /// </summary>
    /// <param name="torrent">The QueuedTorrent to map.</param>
    /// <returns>A new TorrentInfoResult containing the mapped data.</returns>
    private TorrentInfoResult MapQueuedTorrentToTorrentInfo(QueuedDownload torrent)
    {
        return new TorrentInfoResult
        {
            Id = torrent.Id,
            Hash = torrent.Hash,
            Name = torrent.Name,
            Magnet = torrent.Magnet,
            CreatedAt = torrent.CreatedAt,
            DownloadState = "queued",
            TorrentFile = torrent.TorrentFile != null,
            Progress = 0.0,
            Files = [],
            DownloadSpeed = 0,
            Seeds = 0,
            UpdatedAt = torrent.CreatedAt
        };
    }


    /// <inheritdoc />
    public async Task<TorrentInfoResult?> GetIdInfoAsync(int id, bool skipCache = false, int limit = 1000, CancellationToken cancellationToken = default)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["id"] = id.ToString();
        parameters["bypass_cache"] = skipCache.ToString().ToLowerInvariant();
        parameters["limit"] = limit.ToString();

        var currentTorrent = await _requests.GetRequestAsync($"torrents/mylist?{parameters}", true, cancellationToken);

        if (currentTorrent != null)
        {
            var response = JsonConvert.DeserializeObject<Response<TorrentInfoResult?>>(currentTorrent);
            var torrent = response?.Data;

            if (torrent != null)
            {
                return torrent;
            }
        }

        var queuedTorrent = await _queued.GetQueuedAsync(skipCache, "torrent", id, 0, limit, cancellationToken);

        if (queuedTorrent is { Count: > 0 })
        {
            return MapQueuedTorrentToTorrentInfo(queuedTorrent[0]);
        }

        return null;
    }

    /// <inheritdoc />
    [Obsolete("Torrents.GetHashInfoAsync will be removed on or after 31 August 2026. Migrate to Torrents.GetIdInfoAsync and use torrent ID instead of torrent hash.")]
    public async Task<TorrentInfoResult?> GetHashInfoAsync(string hash, bool skipCache = false, int limit = 1000, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Deprecation notice: Torrents.GetHashInfoAsync will be removed on or after 31 August 2026. Migrate to Torrents.GetIdInfoAsync and use torrent ID instead of torrent hash.");

        return await GetHashInfoCoreAsync(hash, skipCache, limit, cancellationToken);
    }

    private async Task<TorrentInfoResult?> GetHashInfoCoreAsync(string hash, bool skipCache = false, int limit = 1000, CancellationToken cancellationToken = default)
    {
        var currentTorrents = await GetCurrentAsync(skipCache, limit, cancellationToken);

        if (currentTorrents != null)
        {
            foreach (var torrent in currentTorrents)
            {
                if (torrent.Hash.Equals(hash, StringComparison.OrdinalIgnoreCase))
                {
                    return torrent;
                }
            }
        }

        var queuedTorrents = await GetQueuedAsync(skipCache, cancellationToken);

        if (queuedTorrents != null)
        {
            foreach (var torrent in queuedTorrents)
            {
                if (torrent.Hash.Equals(hash, StringComparison.OrdinalIgnoreCase))
                {
                    return torrent;
                }
            }
        }

        return null;
    }

    /// <inheritdoc />
    public async Task<Response<TorrentAddResult>> AddFileAsync(Byte[] file, int seeding = 1, bool allowZip = false, string? name = null, bool as_queued = false, CancellationToken cancellationToken = default)
    {
        return await AddFileAsync(file, seeding, allowZip, name, as_queued, false, false, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Response<TorrentAddResult>> AddFileAsync(Byte[] file, int seeding, bool allowZip, string? name, bool as_queued, bool add_only_if_cached, bool async_create, CancellationToken cancellationToken = default)
    {
        using (var content = new MultipartFormDataContent())
        {
            var fileContent = new ByteArrayContent(file);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-bittorrent");

            content.Add(fileContent, "file", "torrent.torrent");
            content.Add(new StringContent(seeding.ToString()), "seed");
            content.Add(new StringContent(allowZip.ToString().ToLowerInvariant()), "allow_zip");
            content.Add(new StringContent(as_queued.ToString().ToLowerInvariant()), "as_queued");
            content.Add(new StringContent(add_only_if_cached.ToString().ToLowerInvariant()), "add_only_if_cached");

            if (name != null)
            {
                content.Add(new StringContent(name), "name");
            }

            var endpoint = async_create ? "torrents/asynccreatetorrent" : "torrents/createtorrent";

            return await _requests.PostRequestMultipartAsync<Response<TorrentAddResult>>(endpoint, content, true, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<Response<TorrentAddResult>> AddMagnetAsync(string magnet, int seeding = 1, bool allowZip = false, string? name = null, bool as_queued = false, CancellationToken cancellationToken = default)
    {
        return await AddMagnetAsync(magnet, seeding, allowZip, name, as_queued, false, false, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Response<TorrentAddResult>> AddMagnetAsync(string magnet, int seeding, bool allowZip, string? name, bool as_queued, bool add_only_if_cached, bool async_create, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(magnet), "magnet");
        content.Add(new StringContent(seeding.ToString()), "seed");
        content.Add(new StringContent(allowZip.ToString().ToLowerInvariant()), "allow_zip");
        content.Add(new StringContent(as_queued.ToString().ToLowerInvariant()), "as_queued");
        content.Add(new StringContent(add_only_if_cached.ToString().ToLowerInvariant()), "add_only_if_cached");

        if (name != null)
        {
            content.Add(new StringContent(name), "name");
        }

        var endpoint = async_create ? "torrents/asynccreatetorrent" : "torrents/createtorrent";

        return await _requests.PostRequestMultipartAsync<Response<TorrentAddResult>>(endpoint, content, true, cancellationToken);
    }

    /// <inheritdoc />
    [Obsolete("Torrents.ControlAsync currently accepts a torrent hash, but on or after 31 August 2026 it will require a torrentId instead. Migrate to Torrents.ControlByIdAsync.")]
    public async Task<Response> ControlAsync(string hash, string action, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Deprecation notice: Torrents.ControlAsync currently accepts a torrent hash, but on or after 31 August 2026 it will require a torrentId instead. Migrate to Torrents.ControlByIdAsync.");

        var info = await GetHashInfoCoreAsync(hash, skipCache: true, cancellationToken: cancellationToken);
        if (info == null)
        {
            throw new TorBoxException("ITEM_NOT_FOUND", $"Torrent with hash {hash} was not found.");
        }

        Object data = info.DownloadState == "queued"
            ? new
            {
                queued_id = (Int32?)info.Id,
                operation = action
            }
            : new
            {
                torrent_id = (Int32?)info.Id,
                operation = action
            };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        string endpoint = info.DownloadState == "queued" ? "torrents/controlqueued" : "torrents/controltorrent";
        return await _requests.PostRequestRawAsync<Response>(endpoint, jsonContent, true, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Response> ControlByIdAsync(int torrentId, string action, CancellationToken cancellationToken = default)
    {
        var info = await GetIdInfoAsync(torrentId, skipCache: true, cancellationToken: cancellationToken);
        var isQueued = info?.DownloadState == "queued";

        Object data = isQueued
            ? new
            {
                queued_id = (Int32?)torrentId,
                operation = action
            }
            : new
            {
                torrent_id = (Int32?)torrentId,
                operation = action
            };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        string endpoint = isQueued ? "torrents/controlqueued" : "torrents/controltorrent";
        return await _requests.PostRequestRawAsync<Response>(endpoint, jsonContent, true, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Response<List<AvailableTorrent?>>> GetAvailabilityAsync(string hash, bool listFiles = false, CancellationToken cancellationToken = default)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["hash"] = hash;
        parameters["format"] = "list";
        parameters["list_files"] = listFiles.ToString().ToLowerInvariant();

        return await _requests.GetRequestAsync<Response<List<AvailableTorrent?>>>($"torrents/checkcached?{parameters}", true, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Response<string>> RequestDownloadAsync(int torrent_id, int? file_id, bool zip = false, CancellationToken cancellationToken = default)
    {
        return await RequestDownloadAsync(torrent_id, file_id, zip, null, false, false, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Response<string>> RequestDownloadAsync(int torrent_id, int? file_id, bool zip, string? user_ip, bool redirect, bool append_name, CancellationToken cancellationToken = default)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["token"] = _store.BearerToken;
        parameters["torrent_id"] = torrent_id.ToString();
        parameters["file_id"] = file_id?.ToString() ?? "0";
        parameters["zip_link"] = zip.ToString().ToLowerInvariant();
        parameters["redirect"] = redirect.ToString().ToLowerInvariant();
        parameters["append_name"] = append_name.ToString().ToLowerInvariant();

        if (!String.IsNullOrWhiteSpace(user_ip))
        {
            parameters["user_ip"] = user_ip;
        }

        return await _requests.GetRequestAsync<Response<String>>($"torrents/requestdl?{parameters}", true, cancellationToken);
    }
}
