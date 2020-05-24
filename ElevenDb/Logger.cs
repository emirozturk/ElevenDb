using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ElevenDb
{
    internal class Logger
    {
        private static readonly List<string> lines = new List<string>();
        private static string logPath;
        public static int MaxLogSizeInKb { get; set; }
        public static string LogPath { set { logPath = Path.Combine(value) + ".log"; File.Create(logPath).Close(); } }
        public static void LogLines(string Method, Result s)
        {
            string output = $"{DateTime.Now:d/M/yyyy HH:mm:ss}|{Method,40}|{s}";
            lines.Add(output);
            if (!s.IsSuccess)
            {
                File.WriteAllText(logPath, Reduce(lines));
            }
            else if (lines.Count == 250)
            {
                File.WriteAllText(logPath, Reduce(lines));
                lines.Clear();
            }
        }
        public static void LogRemaining()
        {
            File.WriteAllText(logPath, Reduce(lines));
        }

        private static string Reduce(List<string> lines)
        {
            string baseString = File.ReadAllText(logPath);
            string newString = string.Join(Environment.NewLine, lines);
            int removeSize = baseString.Length + newString.Length - MaxLogSizeInKb * 1024;
            if (removeSize < 0)
            {
                removeSize = 0;
            }

            return new string(new StringBuilder(baseString).Append(Environment.NewLine).Append(newString).ToString().Skip(removeSize - 1).ToArray()) + Environment.NewLine;
        }
    }
}