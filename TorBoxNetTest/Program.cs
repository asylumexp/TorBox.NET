using TorBoxNET;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;
using System.Collections;
using System.Text;

var rdt = new TorBoxNetClient();

Console.Clear();

Console.WriteLine(await rdt.Torrents.GetInfoAsync("8b42bd038969c2229f8652b59a596caf41e8b703"));
