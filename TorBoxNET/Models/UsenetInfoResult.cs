using Newtonsoft.Json;

namespace TorBoxNET
{
    public class UsenetInfoResult
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("auth_id")]
        public Guid AuthId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("hash")]
        public string Hash { get; set; } = null!;

        [JsonProperty("download_state")]
        public string DownloadState { get; set; } = null!;

        [JsonProperty("download_speed")]
        public long DownloadSpeed { get; set; }

        [JsonProperty("original_url")]
        public string OriginalUrl { get; set; } = null!;

        [JsonProperty("eta")]
        public long Eta { get; set; }

        [JsonProperty("progress")]
        public long Progress { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("download_id")]
        public string DownloadId { get; set; } = null!;

        [JsonProperty("files")]
        public UsenetInfoResultFile[]? Files { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("cached")]
        public bool Cached { get; set; }

        [JsonProperty("download_present")]
        public bool DownloadPresent { get; set; }

        [JsonProperty("download_finished")]
        public bool DownloadFinished { get; set; }

        [JsonProperty("expires_at")]
        public DateTimeOffset ExpiresAt { get; set; }
    }

    public class UsenetInfoResultFile
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("md5")]
        public string Md5 { get; set; } = null!;

        [JsonProperty("hash")]
        public string Hash { get; set; } = null!;

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("s3_path")]
        public string S3Path { get; set; } = null!;

        [JsonProperty("mimetype")]
        public string Mimetype { get; set; } = null!;

        [JsonProperty("short_name")]
        public string ShortName { get; set; } = null!;

        [JsonProperty("absolute_path")]
        public string AbsolutePath { get; set; } = null!;
    }
}
