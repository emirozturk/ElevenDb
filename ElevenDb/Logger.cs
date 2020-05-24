using System;
using System.IO;

namespace ElevenDb
{
    internal class Logger
    {
        private static string logPath;
        public static string LogPath { set => logPath = Path.Combine(value) + ".log"; }
        public static void LogLine(string Method, Result s)
        {
            string output = String.Format("{0:d/M/yyyy HH:mm:ss} | {1} | {2}", DateTime.Now, Method.PadRight(30, ' '), s.ToString());
            File.AppendAllLines(logPath, new string[1] { output });
        }
    }
}