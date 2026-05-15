using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace TorBoxNET.Test;

public static class Setup
{
    private const string SecretsFileName = "secret.json";
    private static readonly Lazy<TestSecrets> Secrets = new(LoadSecrets);

    public static string API_KEY
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Secrets.Value.ApiKey))
            {
                throw new InvalidOperationException($"{SecretsFileName} must contain an apiKey value.");
            }

            return Secrets.Value.ApiKey;
        }
    }

    public static string WebDownloadLink
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Secrets.Value.WebDownloadLink))
            {
                throw new InvalidOperationException($"{SecretsFileName} must contain a webDownloadLink value.");
            }

            return Secrets.Value.WebDownloadLink;
        }
    }

    public static string WebDownloadName
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(Secrets.Value.WebDownloadName))
            {
                return Secrets.Value.WebDownloadName;
            }

            return DeriveName(WebDownloadLink);
        }
    }

    public static bool HasApiKey => !string.IsNullOrWhiteSpace(Secrets.Value.ApiKey);

    public static bool HasWebDownloadLink =>
        HasApiKey && !string.IsNullOrWhiteSpace(Secrets.Value.WebDownloadLink);

    private static TestSecrets LoadSecrets()
    {
        if (!File.Exists(SecretsFileName))
        {
            return new TestSecrets();
        }

        var json = File.ReadAllText(SecretsFileName);
        return JsonSerializer.Deserialize<TestSecrets>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new TestSecrets();
    }

    private static string DeriveName(string link)
    {
        if (Uri.TryCreate(link, UriKind.Absolute, out var uri))
        {
            var segment = uri.Segments.LastOrDefault()?.Trim('/');
            if (!string.IsNullOrWhiteSpace(segment))
            {
                return Uri.UnescapeDataString(segment);
            }
        }

        return "web-download-test";
    }

    private sealed class TestSecrets
    {
        public string ApiKey { get; set; }

        public string WebDownloadLink { get; set; }

        public string WebDownloadName { get; set; }
    }
}

public sealed class LiveFactAttribute : FactAttribute
{
    public LiveFactAttribute()
    {
        if (!Setup.HasApiKey)
        {
            Skip = "Live TorBox API test skipped because secret.json is not present or does not contain apiKey.";
        }
    }
}

public sealed class LiveWebDownloadFactAttribute : FactAttribute
{
    public LiveWebDownloadFactAttribute()
    {
        if (!Setup.HasWebDownloadLink)
        {
            Skip = "Live TorBox WebDL test skipped because secret.json must contain apiKey and webDownloadLink.";
        }
    }
}
