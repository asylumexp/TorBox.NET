﻿using Newtonsoft.Json;

namespace TorBoxNET;

public class TorrentHost
{
    /// <summary>
    ///     Host main domain.
    /// </summary>
    [JsonProperty("host")]
    public String? Host { get; set; }

    /// <summary>
    ///     Max split size possible.
    /// </summary>
    [JsonProperty("max_file_size")]
    public String? MaxFileSize { get; set; }
}