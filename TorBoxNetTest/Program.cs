using TorBoxNET;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;
using System.Collections;
using System.Text;

var rdt = new TorBoxNetClient();

var res = await rdt.Torrents.ControlAsync("f84a7252f0e93d2ae23792557b04ec6e1fb07e10", "delete");
Console.WriteLine(res.Success);
