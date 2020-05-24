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
        public static string LogPath { set => logPath = Path.Combine(value) + ".log"; }
        public static void LogLines(string Method, Result s)
        {
            string output = String.Format("{0:d/M/yyyy HH:mm:ss}|{1}|{2}", DateTime.Now, Method.PadRight(40, ' '), s.ToString());
            lines.Add(output);
            if (!s.IsSuccess)
                File.WriteAllText(logPath, Reduce(lines));
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
            if (removeSize < 0) removeSize = 0;
            return new string(new StringBuilder(baseString).Append(Environment.NewLine).Append(newString).ToString().Skip(removeSize).ToArray());
        }
    }
}