using System;
using System.IO;

namespace ElevenDb
{
    internal class Logger
    {
        static string logPath;
        public static string LogPath { set { logPath = Path.Combine(value) + "log.txt"; } }
        public static void LogLine(object s)
        {
            File.WriteAllLines(logPath, new string[1] { s.ToString() });
        }
    }
}