using Newtonsoft.Json;

namespace TorBoxNET;

public class User
{
    [JsonProperty("id")]
    public long? Id { get; set; }

    [JsonProperty("auth_id")]
    public Guid? AuthId { get; set; }

    [JsonProperty("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    [JsonProperty("plan")]
    public long? Plan { get; set; }

    [JsonProperty("total_downloaded")]
    public long? TotalDownloaded { get; set; }

    [JsonProperty("customer")]
    public string? Customer { get; set; }

    [JsonProperty("is_subscribed")]
    public bool? IsSubscribed { get; set; }

    [JsonProperty("premium_expires_at")]
    public DateTimeOffset? PremiumExpiresAt { get; set; }

    [JsonProperty("cooldown_until")]
    public DateTimeOffset? CooldownUntil { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("user_referral")]
    public Guid? UserReferral { get; set; }

    [JsonProperty("base_email")]
    public string? BaseEmail { get; set; }

    [JsonProperty("total_bytes_downloaded")]
    public long? TotalBytesDownloaded { get; set; }

    [JsonProperty("total_bytes_uploaded")]
    public long? TotalBytesUploaded { get; set; }

    [JsonProperty("torrents_downloaded")]
    public long? TorrentsDownloaded { get; set; }

    [JsonProperty("web_downloads_downloaded")]
    public long? WebDownloadsDownloaded { get; set; }

    [JsonProperty("usenet_downloads_downloaded")]
    public long? UsenetDownloadsDownloaded { get; set; }

    [JsonProperty("additional_concurrent_slots")]
    public long? AdditionalConcurrentSlots { get; set; }

    [JsonProperty("long_term_seeding")]
    public bool? LongTermSeeding { get; set; }

    [JsonProperty("long_term_storage")]
    public bool? LongTermStorage { get; set; }

    [JsonProperty("settings")]
    public UserSettings? Settings { get; set; }
}

public class UserSettings
{
    [JsonProperty("email_notifications")]
    public bool? EmailNotifications { get; set; }

    [JsonProperty("web_notifications")]
    public bool? WebNotifications { get; set; }

    [JsonProperty("mobile_notifications")]
    public bool? MobileNotifications { get; set; }

    [JsonProperty("rss_notifications")]
    public bool? RssNotifications { get; set; }

    [JsonProperty("download_speed_in_tab")]
    public bool? DownloadSpeedInTab { get; set; }

    [JsonProperty("show_tracker_in_torrent")]
    public bool? ShowTrackerInTorrent { get; set; }

    [JsonProperty("stremio_quality")]
    public long[]? StremioQuality { get; set; }

    [JsonProperty("stremio_resolution")]
    public long[]? StremioResolution { get; set; }

    [JsonProperty("stremio_language")]
    public long[]? StremioLanguage { get; set; }

    [JsonProperty("stremio_cache")]
    public long[]? StremioCache { get; set; }

    [JsonProperty("stremio_size_lower")]
    public long? StremioSizeLower { get; set; }

    [JsonProperty("stremio_size_upper")]
    public long? StremioSizeUpper { get; set; }

    [JsonProperty("google_drive_folder_id")]
    public string? GoogleDriveFolderId { get; set; }

    [JsonProperty("onedrive_save_path")]
    public string? OnedriveSavePath { get; set; }

    [JsonProperty("discord_id")]
    public object? DiscordId { get; set; }

    [JsonProperty("discord_notifications")]
    public bool? DiscordNotifications { get; set; }

    [JsonProperty("stremio_allow_adult")]
    public bool? StremioAllowAdult { get; set; }

    [JsonProperty("webdav_flatten")]
    public bool? WebdavFlatten { get; set; }

    [JsonProperty("stremio_seed_torrents")]
    public long? StremioSeedTorrents { get; set; }

    [JsonProperty("seed_torrents")]
    public long? SeedTorrents { get; set; }

    [JsonProperty("allow_zipped")]
    public bool? AllowZipped { get; set; }

    [JsonProperty("stremio_allow_zipped")]
    public bool? StremioAllowZipped { get; set; }

    [JsonProperty("onefichier_folder_id")]
    public object? OnefichierFolderId { get; set; }

    [JsonProperty("gofile_folder_id")]
    public object? GofileFolderId { get; set; }

    [JsonProperty("jdownloader_notifications")]
    public bool? JdownloaderNotifications { get; set; }

    [JsonProperty("webhook_notifications")]
    public bool? WebhookNotifications { get; set; }

    [JsonProperty("webhook_url")]
    public object? WebhookUrl { get; set; }

    [JsonProperty("telegram_notifications")]
    public bool? TelegramNotifications { get; set; }

    [JsonProperty("telegram_id")]
    public object? TelegramId { get; set; }

    [JsonProperty("mega_email")]
    public object? MegaEmail { get; set; }

    [JsonProperty("mega_password")]
    public object? MegaPassword { get; set; }
}