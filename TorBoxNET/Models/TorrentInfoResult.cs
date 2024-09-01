using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TorBoxNET
{
    public class TorrentInfoResult
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("auth_id")]
        public string AuthId { get; set; } = null!;

        [JsonProperty("server")]
        public int Server { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; } = null!;

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("magnet")]
        public string Magnet { get; set; } = null!;

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("download_state")]
        public string DownloadState { get; set; } = null!;

        [JsonProperty("seeds")]
        public int Seeds { get; set; }

        [JsonProperty("peers")]
        public int Peers { get; set; }

        [JsonProperty("ratio")]
        public double Ratio { get; set; }

        [JsonProperty("progress")]
        public double Progress { get; set; }

        [JsonProperty("download_speed")]
        public int DownloadSpeed { get; set; }

        [JsonProperty("upload_speed")]
        public int UploadSpeed { get; set; }

        [JsonProperty("eta")]
        public int Eta { get; set; }

        [JsonProperty("torrent_file")]
        public bool TorrentFile { get; set; }

        [JsonProperty("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        [JsonProperty("download_present")]
        public bool DownloadPresent { get; set; }

        [JsonProperty("files")]
        public List<TorrentInfoResultFile> Files { get; set; } = null!;

        [JsonProperty("download_path")]
        public string DownloadPath { get; set; } = null!;

        [JsonProperty("inactive_check")]
        public int InactiveCheck { get; set; }

        [JsonProperty("availability")]
        public float Availability { get; set; }

        [JsonProperty("download_finished")]
        public bool DownloadFinished { get; set; }

        [JsonProperty("tracker")]
        public string? Tracker { get; set; }

        [JsonProperty("total_uploaded")]
        public long TotalUploaded { get; set; }

        [JsonProperty("total_downloaded")]
        public long TotalDownloaded { get; set; }

        [JsonProperty("cached")]
        public bool Cached { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; } = null!;

        [JsonProperty("seed_torrent")]
        public bool SeedTorrent { get; set; }

        [JsonProperty("allow_zipped")]
        public bool AllowZipped { get; set; }

        [JsonProperty("long_term_seeding")]
        public bool LongTermSeeding { get; set; }

        [JsonProperty("tracker_message")]
        public string? TrackerMessage { get; set; }
    }

    public class TorrentInfoResultFile
    {
        [JsonProperty("id")]
        public int Id { get; set; }

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
        public string MimeType { get; set; } = null!;

        [JsonProperty("short_name")]
        public string ShortName { get; set; } = null!;

        [JsonProperty("absolute_path")]
        public string AbsolutePath { get; set; } = null!;
    }
}
