using Newtonsoft.Json;

namespace TorBoxNET;

public class WebDownloadInfoResult
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonProperty("auth_id")]
    public string? AuthId { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("hash")]
    public string? Hash { get; set; }

    [JsonProperty("download_state")]
    public string? DownloadState { get; set; }

    [JsonProperty("download_speed")]
    public long DownloadSpeed { get; set; }

    [JsonProperty("original_url")]
    public string? OriginalUrl { get; set; }

    [JsonProperty("eta")]
    public long Eta { get; set; }

    [JsonProperty("progress")]
    public double Progress { get; set; }

    [JsonProperty("size")]
    public long Size { get; set; }

    [JsonProperty("download_id")]
    public string? DownloadId { get; set; }

    [JsonProperty("files")]
    public List<WebDownloadInfoResultFile>? Files { get; set; }

    [JsonProperty("active")]
    public bool Active { get; set; }

    [JsonProperty("cached")]
    public bool Cached { get; set; }

    [JsonProperty("download_present")]
    public bool DownloadPresent { get; set; }

    [JsonProperty("download_finished")]
    public bool DownloadFinished { get; set; }

    [JsonProperty("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonProperty("error")]
    public string? Error { get; set; }

    [JsonProperty("cached_at")]
    public DateTimeOffset? CachedAt { get; set; }

    [JsonProperty("server")]
    public int? Server { get; set; }

    [JsonProperty("alternative_hashes")]
    public List<string>? AlternativeHashes { get; set; }

    [JsonProperty("tags")]
    public List<string>? Tags { get; set; }
}

public class WebDownloadInfoResultFile
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("md5")]
    public string? Md5 { get; set; }

    [JsonProperty("hash")]
    public string? Hash { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("size")]
    public long Size { get; set; }

    [JsonProperty("zipped")]
    public bool Zipped { get; set; }

    [JsonProperty("s3_path")]
    public string? S3Path { get; set; }

    [JsonProperty("infected")]
    public bool Infected { get; set; }

    [JsonProperty("mimetype")]
    public string? MimeType { get; set; }

    [JsonProperty("short_name")]
    public string? ShortName { get; set; }

    [JsonProperty("absolute_path")]
    public string? AbsolutePath { get; set; }

    [JsonProperty("opensubtitles_hash")]
    public string? OpenSubtitlesHash { get; set; }
}
