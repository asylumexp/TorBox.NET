using TorBoxNET;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;
using System.Collections;
using System.Text;

var rdt = new TorBoxNetClient();

Console.Clear();


var a = await rdt.Torrents.ControlAsync(77927, "resume");


Console.WriteLine(a.Success);

Console.WriteLine(a.Error);
Console.WriteLine(a.Detail);
