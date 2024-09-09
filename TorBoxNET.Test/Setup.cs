using System;
using System.IO;

namespace TorBoxNET.Test;

public static class Setup
{
    public static String API_KEY => File.ReadAllText("secret.txt");
}