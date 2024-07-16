using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using AvailableFiles = System.Collections.Generic.Dictionary<System.String, System.Collections.Generic.Dictionary<System.String,
    System.Collections.Generic.List<System.Collections.Generic.Dictionary<System.String, TorBoxNET.TorrentInstantAvailabilityFile>>>>;
using AvailableFiles2 = System.Collections.Generic.Dictionary<System.String, System.Collections.Generic.List<System.Collections.Generic.Dictionary<System.String, TorBoxNET.TorrentInstantAvailabilityFile>>>;

namespace TorBoxNET;

public class TorrentsApi
{
    private readonly Requests _requests;

    internal TorrentsApi(HttpClient httpClient, Store store)
    {
        _requests = new Requests(httpClient, store);
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
        var parameters = new NameValueCollection();

        if (skipCache)
        {
            parameters.Add("skipCache", skipCache.ToString());
        }

        var result = await _requests.GetRequestAsync($"torrents/mylist{parameters.ToQueryString()}", true, cancellationToken);


        if (result == null)
        {
            return 0;
        }

        var torrents = JsonConvert.DeserializeObject<Response<dynamic>>(result);

        return torrents?.Data?.Count ?? 0;
    }

    /// <summary>
    ///     Get user torrents list by offset.
    /// </summary>
    /// <param name="skipCache">
    ///     Whether to get refresh cached data on server. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>List of torrents.</returns>
    public async Task<List<Torrent>> GetAsync(bool skipCache = false,
                                      CancellationToken cancellationToken = default)
    {
        var list = await _requests.GetRequestAsync("torrents/mylist", true, cancellationToken);

        Console.WriteLine(list);

        if (list == null)
        {
            return new List<Torrent>();
        }

        var torrentsResponse = JsonConvert.DeserializeObject<Response<Torrent>>(list);
        return torrentsResponse?.Data ?? new List<Torrent>();
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
    public async Task<Torrent?> GetInfoAsync(int id,
                                             bool skipCache = false,
                                             CancellationToken cancellationToken = default)
    {
        var parameters = new NameValueCollection();

        if (skipCache)
        {
            parameters.Add("skipCache", skipCache.ToString());
        }

        var result = await _requests.GetRequestAsync($"torrents/mylist{parameters.ToQueryString()}", true, cancellationToken);

        if (result == null)
        {
            return null;
        }

        var res = JsonConvert.DeserializeObject<Response<Torrent>>(result);

        if (res?.Data != null)
        {
            foreach (var torrent in res.Data)
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
    ///     Get the files available on Real-Debrid for the given torrent.
    /// </summary>
    /// <param name="id">The ID of the torrent</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>List of files available.</returns>
    public async Task<AvailableFiles> GetAvailableFiles(String id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _requests.GetRequestAsync<AvailableFiles>($"torrents/instantAvailability/{id}", true, cancellationToken);
        }
        catch (JsonSerializationException)
        {
            var result = await _requests.GetRequestAsync<AvailableFiles2>($"torrents/instantAvailability/{id}", true, cancellationToken);

            return result.ToDictionary(r => r.Key,
                                       r => new Dictionary<String, List<Dictionary<String, TorrentInstantAvailabilityFile>>>
                                       {
                                           {
                                               "rd", r.Value
                                           }
                                       });
        }
        catch
        {
            return new AvailableFiles();
        }
    }

    /// <summary>
    ///     Get currently active torrents number and the current maximum limit.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>Torrent limits and active count.</returns>
    public async Task<TorrentActiveCount> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await _requests.GetRequestAsync<TorrentActiveCount>("torrents/activeCount", true, cancellationToken);
    }

    /// <summary>
    ///     Get available hosts to upload the torrent to.
    /// </summary>
    /// <returns>List of available hosters.</returns>
    public async Task<IList<TorrentHost>> GetAvailableHostsAsync(CancellationToken cancellationToken = default)
    {
        return await _requests.GetRequestAsync<List<TorrentHost>>("torrents/availableHosts", true, cancellationToken);
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
    public async Task<TorrentAddResult> AddFileAsync(Byte[] file, CancellationToken cancellationToken = default)
    {
        return await _requests.PutRequestAsync<TorrentAddResult>("torrents/addTorrent", file, true, cancellationToken);
    }

    /// <summary>
    ///     Add a magnet link to add to the torrent client.
    /// </summary>
    /// <param name="magnet">Magnet link</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>Info about the added torrent.</returns>
    public async Task<TorrentAddResult> AddMagnetAsync(String magnet, CancellationToken cancellationToken = default)
    {
        var data = new[]
        {
            new KeyValuePair<String, String?>("magnet", magnet)
        };

        return await _requests.PostRequestAsync<TorrentAddResult>("torrents/addMagnet", data, true, cancellationToken);
    }

    /// <summary>
    ///     Select files of a torrent to start the torrent.
    /// </summary>
    /// <param name="id">The ID of the torrent</param>
    /// <param name="fileIds">Selected files IDs or "all"</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns></returns>
    public async Task SelectFilesAsync(String id, String[] fileIds, CancellationToken cancellationToken = default)
    {
        var files = String.Join(",", fileIds);

        var data = new[]
        {
            new KeyValuePair<String, String?>("files", files)
        };

        await _requests.PostRequestAsync($"torrents/selectFiles/{id}", data, true, cancellationToken);
    }

    /// <summary>
    ///     Delete a torrent from torrents list.
    /// </summary>
    /// <param name="id">The ID of the torrent</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns></returns>
    public async Task DeleteAsync(String id, CancellationToken cancellationToken = default)
    {
        await _requests.DeleteRequestAsync($"torrents/delete/{id}", true, cancellationToken);
    }
}