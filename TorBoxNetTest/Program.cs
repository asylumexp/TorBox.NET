using TorBoxNET;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
var rdt = new TorBoxNetClient();

Console.Clear();


var a = await rdt.Torrents.GetInfoAsync(77617, true);

Console.WriteLine(a);
