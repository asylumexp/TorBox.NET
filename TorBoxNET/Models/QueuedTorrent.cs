using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TorBoxNET
{
    public class QueuedTorrent
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("auth_id")]
        public string AuthId { get; set; } = null!;

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("magnet")]
        public string Magnet { get; set; } = null!;

        [JsonProperty("torrent_file")]
        public bool TorrentFile { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; } = null!;

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("type")]
        public string Type { get; set; } = null!;

    }
}
