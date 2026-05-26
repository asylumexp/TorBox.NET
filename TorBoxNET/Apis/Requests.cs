using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace TorBoxNET;

internal class Requests
{
    private readonly HttpClient _httpClient;
    private readonly Store _store;

    public Requests(HttpClient httpClient, Store store)
    {
        _httpClient = httpClient;
        _store = store;
    }

    private async Task<(String? Text, String? HeaderValue)> Request(String baseUrl,
                                                                    String url, 
                                                                    String? headerOutput,
                                                                    Boolean requireAuthentication, 
                                                                    RequestType requestType,
                                                                    HttpContent? data,
                                                                    CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Remove("Authorization");

        if (requireAuthentication)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_store.BearerToken}");
        }

        var retryCount = 0;
        while (true)
        {
            try
            {
                var response = requestType switch
                {
                    RequestType.Get => await _httpClient.GetAsync($"{baseUrl}{url}", cancellationToken),
                    RequestType.Post => await _httpClient.PostAsync($"{baseUrl}{url}", data, cancellationToken),
                    RequestType.Put => await _httpClient.PutAsync($"{baseUrl}{url}", data, cancellationToken),
                    RequestType.Delete => await _httpClient.DeleteAsync($"{baseUrl}{url}", cancellationToken),
                    _ => throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null)
                };

                var buffer = await response.Content.ReadAsByteArrayAsync();
                var text = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                if (response.StatusCode == HttpStatusCode.Unauthorized && requireAuthentication && _store.AuthenticationType == AuthenticationType.OAuth2)
                {
                    var torBoxException = ParseTorBoxException(text);

                    if (torBoxException?.Error == "BAD_TOKEN")
                    {
                        throw new AccessTokenExpired();
                    }
                }

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    text = null;
                }

                if (response.StatusCode == (HttpStatusCode)429)
                {
                    throw ParseTorBoxException(text) ?? new TorBoxException("RATE_LIMIT", text);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var torBoxException = ParseTorBoxException(text);

                    if (torBoxException != null)
                    {
                        throw torBoxException;
                    }
                    else
                    {
                        throw new Exception(text);
                    }
                }

                var apiException = ParseTorBoxException(text);

                if (apiException != null)
                {
                    throw apiException;
                }

                if (headerOutput != null)
                {
                    response.Headers.TryGetValues(headerOutput, out var headerValues);

                    var headerValue = headerValues?.FirstOrDefault();

                    return (text, headerValue);
                }

                return (text, null);
            }
            catch (TorBoxException ex) when (ShouldRetry(ex, retryCount))
            {
                retryCount++;

                await DelayBeforeRetry(retryCount, cancellationToken);
            }
            catch (TorBoxException)
            {
                throw;
            }
            catch
            {
                if (retryCount >= _store.RetryCount)
                {
                    throw;
                }

                retryCount++;

                await DelayBeforeRetry(retryCount, cancellationToken);
            }
        }
    }
        
    private async Task<T> Request<T>(String baseUrl,
                                     String url,
                                     Boolean requireAuthentication,
                                     RequestType requestType,
                                     HttpContent? data,
                                     CancellationToken cancellationToken)
        where T : class, new()
    {
        var (result, _) = await Request(baseUrl, url, null, requireAuthentication, requestType, data, cancellationToken);

        if (result == null)
        {
            return new T();
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(result) ?? throw new JsonSerializationException();
        }
        catch (JsonSerializationException ex)
        {
            throw new JsonSerializationException($"Unable to deserialize Real Debrid API response to {typeof(T).Name}. Response was: {result}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to deserialize Real Debrid API response to {typeof(T).Name}. Response was: {result}", ex);
        }
    }

    public async Task<T> GetAuthRequestAsync<T>(String url, CancellationToken cancellationToken)
        where T : class, new()
    {
        return await Request<T>(Store.AuthUrl, url, false, RequestType.Get, null, cancellationToken);
    }

    public async Task<T> PostAuthRequestAsync<T>(String url, IEnumerable<KeyValuePair<String, String?>> data, CancellationToken cancellationToken)
        where T : class, new()
    {
        var content = new FormUrlEncodedContent(data);
        return await Request<T>(Store.AuthUrl, url, false, RequestType.Post, content, cancellationToken);
    }

    public async Task<String?> GetRequestHeaderAsync(String url,
                                                     String header,
                                                     Boolean requireAuthentication,
                                                     CancellationToken cancellationToken)
    {
        var (_, headerValue) = await Request(Store.ApiUrl, url, header, requireAuthentication, RequestType.Get, null, cancellationToken);

        return headerValue;
    }

    public async Task<String?> GetRequestAsync(String url, Boolean requireAuthentication, CancellationToken cancellationToken)
    {
        var (text, _) = await Request(Store.ApiUrl, url, null, requireAuthentication, RequestType.Get, null, cancellationToken);

        return text;
    }

    public async Task<T> GetRequestAsync<T>(String url, Boolean requireAuthentication, CancellationToken cancellationToken)
        where T : class, new()
    {
        return await Request<T>(Store.ApiUrl, url, requireAuthentication, RequestType.Get, null, cancellationToken);
    }
    public async Task<T> GetLinkRequestAsync<T>(String url, Boolean requireAuthentication, CancellationToken cancellationToken)
    where T : class, new()
    {
        url += $"&token={_store.BearerToken}";
        return await Request<T>(Store.ApiUrl, url, requireAuthentication, RequestType.Get, null, cancellationToken);
    }

    public async Task PostRequestAsync(String url, IEnumerable<KeyValuePair<String, String?>>? data, Boolean requireAuthentication, CancellationToken cancellationToken)
    {
        var content = data != null ? new FormUrlEncodedContent(data) : null;
        await Request(Store.ApiUrl, url, null, requireAuthentication, RequestType.Post, content, cancellationToken);
    }

    public async Task PostRequestRawAsync(String url, HttpContent? data, Boolean requireAuthentication, CancellationToken cancellationToken)
    {
        await Request(Store.ApiUrl, url, null, requireAuthentication, RequestType.Post, data, cancellationToken);
    }
    public async Task<T> PostRequestRawAsync<T>(String url, HttpContent? data, Boolean requireAuthentication, CancellationToken cancellationToken)
    where T : class, new()
    {
        return await Request<T>(Store.ApiUrl, url, requireAuthentication, RequestType.Post, data, cancellationToken);
    }

    public async Task<T> PutRequestRawAsync<T>(String url, HttpContent? data, Boolean requireAuthentication, CancellationToken cancellationToken)
    where T : class, new()
    {
        return await Request<T>(Store.ApiUrl, url, requireAuthentication, RequestType.Put, data, cancellationToken);
    }
    public async Task<T> PostRequestAsync<T>(String url, IEnumerable<KeyValuePair<String, String?>>? data, Boolean requireAuthentication, CancellationToken cancellationToken)
        where T : class, new()
    {
        var content = data != null ? new FormUrlEncodedContent(data) : null;
        return await Request<T>(Store.ApiUrl, url, requireAuthentication, RequestType.Post, content, cancellationToken);
    }

    public async Task<T> PostRequestMultipartAsync<T>(String url, MultipartFormDataContent? data, Boolean requireAuthentication, CancellationToken cancellationToken)
    where T : class, new()
    {
        return await Request<T>(Store.ApiUrl, url, requireAuthentication, RequestType.Post, data, cancellationToken);
    }

    public async Task PutRequestAsync(String url, Byte[] file, Boolean requireAuthentication, CancellationToken cancellationToken)
    {
        var content = new ByteArrayContent(file);
        await Request(Store.ApiUrl, url, null, requireAuthentication, RequestType.Put, content, cancellationToken);
    }

    public async Task<T> PutRequestAsync<T>(String url, Byte[] file, Boolean requireAuthentication, CancellationToken cancellationToken)
        where T : class, new()
    {
        var content = new ByteArrayContent(file);
        return await Request<T>(Store.ApiUrl, url, requireAuthentication, RequestType.Put, content, cancellationToken);
    }

    public async Task DeleteRequestAsync(String url, Boolean requireAuthentication, CancellationToken cancellationToken)
    {
        await Request(Store.ApiUrl, url, null, requireAuthentication, RequestType.Delete, null, cancellationToken);
    }

    private enum RequestType
    {
        Get,
        Post,
        Put,
        Delete
    }

    private static TorBoxException? ParseTorBoxException(String? text)
    {
        try
        {
            if (text == null)
            {
                return null;
            }

            var requestError = JsonConvert.DeserializeObject<Response>(text);

            var detail = requestError?.Detail;

            if (IsRateLimitDetail(detail))
            {
                return new TorBoxException("RATE_LIMIT", detail);
            }

            if (requestError?.Error != null || requestError?.Success == false)
            {
                return new TorBoxException(requestError.Error ?? "UNKNOWN_ERROR", requestError.Detail);
            }

            return null;
        }
        catch
        {
            return IsRateLimitDetail(text)
                ? new TorBoxException("RATE_LIMIT", text)
                : null;
        }
    }

    private static Boolean IsRateLimitDetail(String? detail)
    {
        if (String.IsNullOrWhiteSpace(detail))
        {
            return false;
        }

        return detail!.IndexOf("rate limit", StringComparison.OrdinalIgnoreCase) >= 0
               || detail.IndexOf("per 1 hour", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private Boolean ShouldRetry(TorBoxException exception, Int32 retryCount)
    {
        return retryCount < _store.RetryCount && IsTransientTorBoxError(exception.Error);
    }

    private static Boolean IsTransientTorBoxError(String error)
    {
        return error.Equals("DATABASE_ERROR", StringComparison.OrdinalIgnoreCase)
               || error.Equals("DOWNLOAD_SERVER_ERROR", StringComparison.OrdinalIgnoreCase)
               || error.Equals("UNKNOWN_ERROR", StringComparison.OrdinalIgnoreCase)
               || error.Equals("NO_SERVERS_AVAILABLE_ERROR", StringComparison.OrdinalIgnoreCase);
    }

    private async Task DelayBeforeRetry(Int32 retryCount, CancellationToken cancellationToken)
    {
        var configuredRetryLimit = _store.RetryCount <= 0 ? 1 : _store.RetryCount;
        var boundedRetryCount = Math.Min(retryCount, configuredRetryLimit);
        var delayMs = Math.Min(30000, 1000 * (Int32)Math.Pow(2, boundedRetryCount - 1));

        await Task.Delay(delayMs, cancellationToken);
    }
}
