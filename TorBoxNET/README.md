# TorBoxNET: A .NET wrapper for the TorBox API

Forked from [rogerfar's RDNET](https://github.com/rogerfar/RD.NET).

Currently supports all Torrents, Usenet, WebDL API calls.

## Deprecation notices

`GetHashInfoAsync` is deprecated. On or after 31 August 2026, it will be removed. Migrate to `GetIdInfoAsync` and use torrent ID instead of torrent hash.

`ControlAsync` currently accepts a torrent hash. On or after 31 August 2026, it will require a `torrentId` instead. Migrate to `ControlByIdAsync`.

On or after 31 August 2026, `ControlByIdAsync` is expected to become an alias for `ControlAsync` after `ControlAsync` changes to require a `torrentId`.
