using TorBoxNET;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
var rdt = new TorBoxNetClient();


Console.WriteLine(await rdt.Torrents.GetTotal(true));
