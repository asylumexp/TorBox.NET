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
            _ => error
        };
    }
}