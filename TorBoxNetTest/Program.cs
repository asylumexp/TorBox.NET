using TorBoxNET;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;
using System.Collections;
using System.Text;

var rdt = new TorBoxNetClient();

Console.Clear();


var mag = "magnet:?xt=urn:btih:CLQ5KFLERJAWTPNZXCLMMQHN7WEZPZIG&dn=An.English.Haunting-TENOKE&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337";
var b = await rdt.Torrents.AddMagnetAsync(mag, 3, false, "torrent pls");

Console.WriteLine(b.Success);
Console.WriteLine(b.Error);
var fileBytes = await File.ReadAllBytesAsync("C:\\torrent.torrent");

var a = await rdt.Torrents.AddFileAsync(fileBytes, 3, false, "torentfile");


Console.WriteLine(a.Success);

Console.WriteLine(a.Error);
