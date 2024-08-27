﻿namespace TorBoxNET;

public class UnrestrictApi
{
    private readonly Requests _requests;

    internal UnrestrictApi(HttpClient httpClient, Store store)
    {
        _requests = new Requests(httpClient, store);
    }

    /// <summary>
    ///     Unrestrict a hoster link and get a new unrestricted link
    /// </summary>
    /// <param name="link">The link to the hoster file</param>
    /// <param name="password">Optional password</param>
    /// <param name="remote">True to use remote data</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>Information about the link that was unrestricted.</returns>
    public async Task<Response<String>> LinkAsync(String torrentList,
                                                CancellationToken cancellationToken = default)
    {
        var torrentFile = torrentList.ToList()!;
        return await _requests.GetRequestAsync<Response<String>>($"torrents/requestdl?torrent_id={torrentFile[0]}&file_id={torrentFile[1]}", true, cancellationToken);
    }

    /// <summary>
    ///     Unrestrict a hoster folder link and get individual links, returns an empty array if no links found.
    /// </summary>
    /// <param name="link">The link to the host folder.</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>A list of links in the folder.</returns>
    public async Task<IList<String>> FolderAsync(String link, CancellationToken cancellationToken = default)
    {
        var data = new[]
        {
            new KeyValuePair<String, String?>("link", link)
        };

        return await _requests.PostRequestAsync<List<String>>("unrestrict/folder", data, true, cancellationToken);
    }

    /// <summary>
    ///     Decrypt a container file (RSDF, CCF, CCF3, DLC).
    /// </summary>
    /// <param name="fileContents">The file contents of the container file.</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>A list of URL's</returns>
    public async Task<IList<String>> ContainerFileAsync(Byte[] fileContents, CancellationToken cancellationToken = default)
    {
        return await _requests.PutRequestAsync<List<String>>("unrestrict/containerFile", fileContents, true, cancellationToken);
    }

    /// <summary>
    ///     Decrypt a container file from a link.
    /// </summary>
    /// <param name="link">HTTP Link of the container file.</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>A list of URL's</returns>
    public async Task<IList<String>> ContainerLinkAsync(String link, CancellationToken cancellationToken = default)
    {
        var data = new[]
        {
            new KeyValuePair<String, String?>("link", link)
        };

        return await _requests.PostRequestAsync<List<String>>("unrestrict/containerLink", data, true, cancellationToken);
    }
}