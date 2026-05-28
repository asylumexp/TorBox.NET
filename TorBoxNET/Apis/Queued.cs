using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using System.Xml.Linq;

namespace TorBoxNET;

public interface IQueuedApi
{
    /// <summary>
    /// Gets the list of user's queued downloads. If an ID is supplied, returns a list containing one download.
    /// </summary>
    /// <param name="skipCache">Whether to bypass the cache and retrieve fresh data from the server. Defaults to false.</param>
    /// <param name="type">Type of requested queued item. Defaults to "torrent".</param>
    /// <param name="id">ID of individual queued item. If supplied, only that torrent is returned.</param>
    /// <param name="offset">Offset for list of items requested. Defaults to 0.</param>
    /// <param name="limit">Limits the number of returned items. Defaults to 1000.</param>
    /// <param name="cancellationToken">A token to cancel the task if necessary.</param>
    /// <returns>A list of TorrentInfoResult, an empty list if nothing is found, null if request failed.</returns>
    Task<List<QueuedDownload>?> GetQueuedAsync(
        bool skipCache = false,
        string type = "torrent",
        int? id = null,
        int offset = 0,
        int limit = 1000,
        CancellationToken cancellationToken = default);
}

public class QueuedApi : IQueuedApi
{
    private readonly Requests _requests;

    internal QueuedApi(HttpClient httpClient, Store store)
    {
        _requests = new Requests(httpClient, store);
    }

    /// <inheritdoc />
    public async Task<List<QueuedDownload>?> GetQueuedAsync(
        bool skipCache = false,
        string type = "torrent",
        int? id = null,
        int offset = 0,
        int limit = 1000,
        CancellationToken cancellationToken = default)
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["type"] = type;
        parameters["bypass_cache"] = skipCache.ToString().ToLowerInvariant();
        if (id.HasValue)
        {
            parameters["id"] = id.Value.ToString();
        }
        parameters["offset"] = offset.ToString();
        parameters["limit"] = limit.ToString();

        var responseString = await _requests.GetRequestAsync($"queued/getqueued?{parameters}", true, cancellationToken);
        if (responseString == null)
        {
            return null;
        }

        if (id.HasValue)
        {
            var queuedDownloads = JsonConvert.DeserializeObject<Response<QueuedDownload>>(responseString);
            if (queuedDownloads?.Data != null)
            {
                return [queuedDownloads.Data];
            }
        }
        else
        {
            var torrentsResponse = JsonConvert.DeserializeObject<Response<List<QueuedDownload>>>(responseString);
            if (torrentsResponse?.Data != null)
            {
                return torrentsResponse.Data;
            }
        }
        return [];
    }
}
