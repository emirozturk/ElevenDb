using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ElevenDb
{
    internal class Logger
    {
        private static readonly List<string> lines = new List<string>();
        private static string logPath;
        public static int MaxLogSizeInKb { get; set; }
        public static string LogPath { set => logPath = Path.Combine(value) + ".log"; }
        public static void LogLines(string Method, Result s)
        {
            string output = String.Format("{0:d/M/yyyy HH:mm:ss} | {1} | {2}", DateTime.Now, Method.PadRight(40, ' '), s.ToString());
            if (!s.IsSuccess)
            {
                File.AppendAllLines(logPath, lines);
                File.AppendAllLines(logPath, new string[1] { output });
            }
            else
                lines.Add(output);
            if (lines.Count == 250)
            {
                File.AppendAllLines(logPath, lines);
                lines.Clear();
            }
        }
        public static void LogRemaining()
        {
            File.AppendAllLines(logPath, lines);
            string log = File.ReadAllText(logPath);
            int removeSize = log.Length - MaxLogSizeInKb * 1024;
            log = new string(log.Skip(removeSize).ToArray());
            File.WriteAllText(logPath,log);
        }
    }
}