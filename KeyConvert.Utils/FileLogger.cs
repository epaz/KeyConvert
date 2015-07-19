using System;
using System.IO;
using System.Text;

namespace KeyConvert.Utils
{
    public class FileLogger : ILogger
    {
        private readonly StringBuilder _logBuilder = new StringBuilder();

        public void Log(LogLevel level, string msg)
        {
            var logMsg = string.Format("{0:HH:mm:ss.FFF} {1}: {2}", DateTime.Now, level.ToString().ToUpper(), msg);
            _logBuilder.AppendLine(logMsg);
        }

        public void Info(string msg)
        {
            Log(LogLevel.Info, msg);
        }

        public void Warn(string msg)
        {
            Log(LogLevel.Warn, msg);
        }

        public void Error(string msg)
        {
            Log(LogLevel.Error, msg);
        }

        public void SaveToFile(string filePath)
        {
            File.WriteAllText(filePath, _logBuilder.ToString());
        }

        public string GetFullLogText()
        {
            return _logBuilder.ToString();
        }
    }
}
