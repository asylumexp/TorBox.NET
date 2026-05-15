using System.Xml.Linq;

namespace TorBoxNET;

public interface IUserApi
{
    /// <summary>
    ///     Returns information about logged in user.
    /// </summary>
    /// <param name="settings">Output user settings</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>
    ///     The currently logged in user.
    /// </returns>
    Task<Response<User>> GetAsync(bool settings, CancellationToken cancellationToken = default);
}

public class UserApi : IUserApi
{
    private readonly Requests _requests;

    internal UserApi(HttpClient httpClient, Store store)
    {
        _requests = new Requests(httpClient, store);
    }

    /// <inheritdoc />
    public async Task<Response<User>> GetAsync(bool settings, CancellationToken cancellationToken = default)
    {
        return await _requests.GetRequestAsync<Response<User>>($"user/me?settings={settings}", true, cancellationToken);
    }

    //public async Task<Response<User>> RefreshAsync(string sessionToken, CancellationToken cancellationToken = default)
    //{
    //    var data = new List<KeyValuePair<string, string?>>
    //{
    //    new("session_token", sessionToken),
    //};
    //    return await _requests.PostRequestAsync<Response<User>>($"user/refreshtoken", data, true, cancellationToken);
    //}
}