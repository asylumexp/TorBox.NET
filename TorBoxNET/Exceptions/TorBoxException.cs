namespace TorBoxNET;

public class TorBoxException : Exception
{
    public TorBoxException(String? error, String? detail)
        : base(GetMessage(error) ?? error)
    {
        ErrorDetail = detail;
        Error = GetMessage(error) ?? "NULL_DETAIL_ERROR";
    }

    public String Error { get; }
    public String? ErrorDetail { get; }

    public static String? GetMessage(String? error)
    {
        return error switch
        {
            "DATABASE_ERROR" => "Could not access internal database/memory store information.",
            "UNKNOWN_ERROR" => "The reason for the error is unknown. Usually, there will be error data attached in the 'data' key. In these cases, please report the request to contact@torbox.app.",
            "NO_AUTH" => "There are no provided credentials.",
            "BAD_TOKEN" => "The provided token is invalid.",
            "AUTH_ERROR" => "There was an error verifying the given authentication.",
            "INVALID_OPTION" => "The provided option is invalid.",
            "REDIRECT_ERROR" => "The server tried redirecting, but it faulted.",
            "OAUTH_VERIFICATION_ERROR" => "The server tried verifying your OAuth token, but it was not accepted by the provider.",
            "ENDPOINT_NOT_FOUND" => "You have hit an endpoint that doesn't exist.",
            "ITEM_NOT_FOUND" => "The item you queried cannot be found.",
            "PLAN_RESTRICTED_FEATURE" => "This feature is restricted to users of higher plans. The user is recommended to upgrade their plan to use this endpoint.",
            "DUPLICATE_ITEM" => "This item already exists.",
            "BOZO_RSS_FEED" => "This RSS feed is invalid or not a well-formed XML.",
            "SELLIX_ERROR" => "There was an error with the Sellix API, usually in the case of payments.",
            "TOO_MUCH_DATA" => "Client sent too much data to the API. Please keep requests under 100MB in size.",
            "MISSING_REQUIRED_OPTION" => "The API is missing required information to process the request.",
            "TOO_MANY_OPTIONS" => "Client sent too many options. Usually this has to do with the API requiring only 1 option but the client sent more than the required.",
            "BOZO_TORRENT" => "The torrent sent is not a valid torrent.",
            "NO_SERVERS_AVAILABLE_ERROR" => "There are no download servers available to handle this request. This should never happen. If you receive this error, please contact us at contact@torbox.app.",
            "MONTHLY_LIMIT" => "User has hit the maximum monthly limit. It is recommended user upgrade their account to be able to download more.",
            "COOLDOWN_LIMIT" => "User is on download cooldown. It is recommended user upgrade their account to bypass this restriction.",
            "ACTIVE_LIMIT" => "User has hit their max active download limit. It is recommended user upgrade their account or purchase addons to bypass this restriction.",
            "DOWNLOAD_SERVER_ERROR" => "There was an error interacting with the download server. It is recommended to wait before trying again.",
            "BOZO_NZB" => "The NZB sent is not a valid NZB file.",
            "SEARCH_ERROR" => "There was an error searching using the TorBox Search API.",
            "INVALID_DEVICE" => "The client is sending requests from the incorrect device.",
            "DIFF_ISSUE" => "The request parameters sent do not allow for this request to complete.",
            "LINK_OFFLINE" => "The link given is inaccessible or has no online files.",
            _ => error
        };

    }
}