using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace TorBoxNET;

/// <summary>
/// Provides methods for interacting with the torrent-related API endpoints, including adding, 
/// retrieving, and controlling torrents, as well as requesting download links.
/// </summary>
public class TorrentsApi
{
    private readonly Requests _requests;
    private readonly Store _store;

    internal TorrentsApi(HttpClient httpClient, Store store)
    {
        _requests = new Requests(httpClient, store);
        _store = store;
    }

    /// <summary>
    /// Retrieves the total number of user torrents.
    /// </summary>
    /// <param name="skipCache">
    /// Whether to bypass the cache and fetch fresh data from the server. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// The total number of torrents, or -1 if the request fails.
    /// </returns>
    public async Task<Int64> GetTotal(bool skipCache = false, CancellationToken cancellationToken = default)
    {
        var res = await GetCurrentAsync(skipCache, cancellationToken);

        if (res == null)
        {
            return -1;
        }

        return res.Count;
    }

    /// <summary>
    /// Fetches the list of active torrents for the user.
    /// </summary>
    /// <param name="skipCache">
    /// Whether to bypass the cache and retrieve fresh data from the server. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// A list of torrents if the request succeeds, otherwise null.
    /// </returns>
    public async Task<List<TorrentInfoResult>?> GetCurrentAsync(bool skipCache = false, CancellationToken cancellationToken = default)
    {
        var list = await _requests.GetRequestAsync($"torrents/mylist?bypass_cache={skipCache}", true, cancellationToken);

        if (list == null)
        {
            return null;
        }

        return JsonConvert.DeserializeObject<Response<List<TorrentInfoResult>>>(list)?.Data;
    }

    /// <summary>
    /// Retrieves a list of torrents currently queued for download.
    /// </summary>
    /// <param name="skipCache">
    /// Whether to bypass the cache and retrieve fresh data from the server. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// A list of queued torrents if the request succeeds, otherwise null.
    /// </returns>
    public async Task<List<TorrentInfoResult>?> GetQueuedAsync(bool skipCache = false, CancellationToken cancellationToken = default)
    {
        var list = await _requests.GetRequestAsync("torrents/getqueued", true, cancellationToken);

        if (list == null)
        {
            return null;
        }

        var queuedTorrents = JsonConvert.DeserializeObject<Response<List<QueuedTorrent>>>(list)?.Data;

        var torrents = queuedTorrents.Select(torrent => new TorrentInfoResult
        {
            Id = torrent.Id,
            AuthId = torrent.AuthId,
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
        }).ToList();

        return torrents;
    }

    /// <summary>
    /// Retrieves detailed information about a specific torrent by its ID.
    /// Checks both active and queued torrents.
    /// </summary>
    /// <param name="id">The unique identifier of the torrent.</param>
    /// <param name="skipCache">
    /// Whether to bypass the cache and retrieve fresh data from the server. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// Information about the torrent if found, otherwise null.
    /// </returns>
    public async Task<TorrentInfoResult?> GetIdInfoAsync(int id, bool skipCache = false, CancellationToken cancellationToken = default)
    {
        var currentTorrent = await _requests.GetRequestAsync($"torrents/mylist?bypass_cache={skipCache}", true, cancellationToken);

        if (currentTorrent != null) 
        {
            var torrent = JsonConvert.DeserializeObject<Response<TorrentInfoResult?>>(currentTorrent)?.Data;
            if (torrent != null)
            {
                return torrent;
            }
        }

        var queuedTorrents = await GetQueuedAsync(skipCache, cancellationToken);

        if (queuedTorrents != null)
        {
            foreach (var torrent in queuedTorrents)
            {
                if (torrent.Id == id)
                {
                    return torrent;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Retrieves detailed information about a specific torrent by its hash.
    /// Checks both active and queued torrents.
    /// </summary>
    /// <param name="hash">The unique hash identifier of the torrent.</param>
    /// <param name="skipCache">
    /// Whether to bypass the cache and retrieve fresh data from the server. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the task if necessary.
    /// </param>
    /// <returns>
    /// Information about the torrent if found, otherwise null.
    /// </returns>
    public async Task<TorrentInfoResult?> GetHashInfoAsync(string hash, bool skipCache = false, CancellationToken cancellationToken = default)
    {
        var currentTorrents = await GetCurrentAsync(skipCache, cancellationToken);

        if (currentTorrents != null)
        {
            foreach (var torrent in currentTorrents)
            {
                if (torrent.Hash == hash)
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
                if (torrent.Hash == hash)
                {
                    return torrent;
                }
            }
        }

        return null;
    }

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
    public async Task<Response<TorrentAddResult>> AddFileAsync(Byte[] file, int seeding = 1, bool allowZip = false, string? name = null, CancellationToken cancellationToken = default)
    {
        using (var content = new MultipartFormDataContent())
        {
            var fileContent = new ByteArrayContent(file);
            fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
            {
                Name = "file",
                FileName = "torrent.torrent"
            };
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-bittorrent");

            content.Add(fileContent);
            content.Add(new StringContent(seeding.ToString()), "seed");
            content.Add(new StringContent(allowZip.ToString()), "allow_zip");

            if (name != null)
            {
                content.Add(new StringContent(name), "name");
            }

            return await _requests.PostRequestMultipartAsync<Response<TorrentAddResult>>("torrents/createtorrent", content, true, cancellationToken);
        }
    }

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
    public async Task<Response<TorrentAddResult>> AddMagnetAsync(string magnet, int seeding = 1, bool allowZip = false, string? name = null, CancellationToken cancellationToken = default)
    {
        var data = new List<KeyValuePair<string, string?>>
        {
            new KeyValuePair<string, string?>("magnet", magnet),
            new KeyValuePair<string, string?>("seed", seeding.ToString()),
            new KeyValuePair<string, string?>("allow_zip", allowZip.ToString()),
            new KeyValuePair<string, string?>("name", name)
        };

        return await _requests.PostRequestAsync<Response<TorrentAddResult>>("torrents/createtorrent", data, true, cancellationToken);
    }

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
    public async Task<Response> ControlAsync(string hash, string action, CancellationToken cancellationToken = default)
    {
        var info = await GetHashInfoAsync(hash, skipCache: true, cancellationToken);
        var data = new
        {
            torrent_id = info!.Id,
            operation = action
        };
        var jsonContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        if (info.DownloadState == "queued")
        {
            return await _requests.PostRequestRawAsync<Response>("torrents/controlqueued", jsonContent, true, cancellationToken);
        }
        else
        {
            return await _requests.PostRequestRawAsync<Response>("torrents/controltorrent", jsonContent, true, cancellationToken);
        }
    }

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
    public async Task<Response<List<AvailableTorrent?>>> GetAvailabilityAsync(string hash, bool listFiles = false, CancellationToken cancellationToken = default)
    {
        return await _requests.GetRequestAsync<Response<List<AvailableTorrent?>>>($"torrents/checkcached?hash={hash}&format=list&list_files={listFiles}", true, cancellationToken);
    }

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
    public async Task<Response<string>> RequestDownloadAsync(int torrent_id, int? file_id, bool zip = false, CancellationToken cancellationToken = default)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["token"] = _store.BearerToken;
        parameters["torrent_id"] = torrent_id.ToString();
        parameters["file_id"] = file_id.ToString();
        parameters["zip_link"] = zip.ToString();

        return await _requests.GetRequestAsync<Response<String>>($"torrents/requestdl?{parameters}", true, cancellationToken);
    }
}
