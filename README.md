# TorBox.NET

TorBox.NET is a .NET wrapper library for the [TorBox API](https://api.torbox.app/docs#/), written in C#.

Forked from [rogerfar's RD.NET](https://github.com/rogerfar/RD.NET).

Currently supports Torrents, Usenet, WebDL, queued downloads, and user API calls.

## Installation

Install the package from NuGet:

```powershell
dotnet add package TorBox.NET
```

## Usage

Create an instance of `TorBoxNetClient` for each user you want to authenticate. If you need to support multiple users, create a new instance whenever you switch users.

```csharp
var client = new TorBoxNetClient();
```

The client follows the TorBox API closely in naming and parameters:

```csharp
var client = new TorBoxNetClient();
client.UseApiAuthentication("my-api-key");

var torrents = await client.Torrents.GetCurrentAsync(skipCache: true);
var webDownloads = await client.WebDownloads.GetCurrentAsync(skipCache: true);
```

## Authentication

### API Token

Call `UseApiAuthentication` with the user's TorBox API key:

```csharp
client.UseApiAuthentication("user API key");
```

You can find your TorBox API key in your TorBox account settings.

## Torrents

Add a magnet link:

```csharp
var result = await client.Torrents.AddMagnetAsync(
    "magnet:?xt=urn:btih:...",
    seeding: 1,
    allowZip: false,
    name: "Example torrent");
```

Add a `.torrent` file:

```csharp
var bytes = await File.ReadAllBytesAsync("example.torrent");
var result = await client.Torrents.AddFileAsync(bytes, seeding: 1);
```

List current torrents:

```csharp
var torrents = await client.Torrents.GetCurrentAsync(skipCache: true);
```

Request a download link after TorBox has cached the torrent:

```csharp
var download = await client.Torrents.RequestDownloadAsync(
    torrent_id: 123,
    file_id: 456,
    zip: false);
```

## Usenet

Add an NZB link:

```csharp
var result = await client.Usenet.AddLinkAsync(
    "https://example.com/file.nzb",
    post_processing: 3,
    name: "Example NZB");
```

Add an NZB file:

```csharp
var bytes = await File.ReadAllBytesAsync("example.nzb");
var result = await client.Usenet.AddFileAsync(bytes, post_processing: 3);
```

Request a download link after TorBox has cached the Usenet download:

```csharp
var download = await client.Usenet.RequestDownloadAsync(
    usenet_id: 123,
    file_id: 0,
    zip: false);
```

## WebDL

Create a WebDL from a direct download or supported hoster link:

```csharp
var result = await client.WebDownloads.AddLinkAsync(
    "https://example.com/file.zip",
    name: "file.zip",
    as_queued: false,
    add_only_if_cached: false);
```

List current WebDL items:

```csharp
var webDownloads = await client.WebDownloads.GetCurrentAsync(skipCache: true);
```

Request a generated download link after TorBox has cached the WebDL:

```csharp
var download = await client.WebDownloads.RequestDownloadAsync(
    web_id: 123,
    file_id: 0,
    zip: false);
```

Check supported WebDL hosters:

```csharp
var hosters = await client.WebDownloads.GetHostersAsync();
```

## Queued Downloads

Queued downloads can be queried directly:

```csharp
var queuedTorrents = await client.Queued.GetQueuedAsync(type: "torrent");
var queuedUsenet = await client.Queued.GetQueuedAsync(type: "usenet");
var queuedWebDl = await client.Queued.GetQueuedAsync(type: "webdl");
```

## Unit Tests

To run tests, create `TorBoxNET.Test/secret.json`. An example file exists at `TorBoxNET.Test/secret.json.example`.

You will need a link from a supported provider to run WebDL tests.