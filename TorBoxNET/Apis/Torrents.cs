using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace TorBoxNET;

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
    ///     Get the number of torrents.
    /// </summary>
    /// <param name="skipCache">
    ///     Whether to get refresh cached data on server. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>The number of torrents.</returns>
    public async Task<Int64> GetTotal(bool skipCache = false,
                                      CancellationToken cancellationToken = default)
    {
        var res = await GetCurrentAsync(skipCache);

        if (res == null)
        {
            return 0;
        }

        return res.Count;
    }

    /// <summary>
    ///     Get user torrents list.
    /// </summary>
    /// <param name="skipCache">
    ///     Whether to get refresh cached data on server. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>List of torrents.</returns>
    public async Task<List<TorrentInfoResult>?> GetCurrentAsync(bool skipCache = false,
                                      CancellationToken cancellationToken = default)
    {
        var list = await _requests.GetRequestAsync($"torrents/mylist?bypass_cache={skipCache}", true, cancellationToken);

        if (list == null)
        {
            return null;
        }

        return JsonConvert.DeserializeObject<Response<List<TorrentInfoResult>>>(list)?.Data;
    }

    public async Task<List<TorrentInfoResult>?> GetQueuedAsync(bool skipCache = false,
                                  CancellationToken cancellationToken = default)
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
    ///     Get information about the torrent.
    /// </summary>
    /// <param name="id">The ID of the torrent</param>
    /// <param name="skipCache">
    ///     Whether to get refresh cached data on server. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>Info about the torrent.</returns>
    public async Task<TorrentInfoResult?> GetInfoAsync(string hash,
                                             bool skipCache = false,
                                             CancellationToken cancellationToken = default)
    {
        var currentTorrents = await GetCurrentAsync(skipCache);

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

        var queuedTorrents = await GetQueuedAsync(skipCache);

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
    ///     Add a torrent file to add to the torrent client.
    /// </summary>
    /// <param name="file">The byte array of the file.</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>Info about the added torrent.</returns>
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
    ///     Add a magnet link to add to the torrent client.
    /// </summary>
    /// <param name="magnet">Magnet link</param>
    /// <param name="seeding">Preference for seeding torrent. 1 is auto. 2 is seed. 3 is don't seed.</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>Info about the added torrent.</returns>
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
    ///     Modify a torrent from torrents list.
    /// </summary>
    /// <param name="id">The ID of the torrent</param>
    /// <param name="action">The action to be performed on the torrent, valid options are: pause, resume, reannounce, delete.</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns></returns>
    public async Task<Response> ControlAsync(string hash, string action, CancellationToken cancellationToken = default)
    {
        var info = await GetInfoAsync(hash, skipCache: true);
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
    ///     Modify a torrent from queued torrents list.
    /// </summary>
    /// <param name="id">The ID of the torrent</param>
    /// <param name="action">The action to be performed on the torrent, valid options are: delete.</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns></returns>
    public async Task<Response<List<AvailableTorrent?>>> GetAvailabilityAsync(string hash, bool listFiles = false, CancellationToken cancellationToken = default)
    {
        return await _requests.GetRequestAsync<Response<List<AvailableTorrent?>>>($"torrents/checkcached?hash={hash}&format=list&list_files={listFiles}", true, cancellationToken);
    }

    /// <summary>
    ///     Request download link for torrent.
    /// </summary>
    /// <param name="torrent_id"> Id of torrent.</param>
    /// <param name="file_id"> File ID for file.</param>
    /// <param name="zip">Whether to zip files, if true then all files are donwloaded, no matter the value of file_id</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns></returns>
    public async Task<Response<String>> RequestDownloadAsync(int torrent_id, int? file_id, bool zip = false, CancellationToken cancellationToken = default)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["token"] = _store.BearerToken;
        parameters["torrent_id"] = torrent_id.ToString();
        parameters["file_id"] = file_id.ToString();
        parameters["zip"] = zip.ToString();

        return await _requests.GetRequestAsync<Response<String>>($"torrents/requestdl?{parameters}", true, cancellationToken);
    }
}