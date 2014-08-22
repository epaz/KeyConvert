using System;
using System.IO;
using System.Text;

namespace KeyConvert.Utils
{
    public class Logger
    {
        private readonly StringBuilder _logBuilder = new StringBuilder();

        public virtual void Log(Level level, string msg)
        {
            string logMsg = string.Format("{0:HH:mm:ss.FFF} {1}: {2}", DateTime.Now, level.ToString().ToUpper(), msg);
            _logBuilder.AppendLine(logMsg);
        }

        public enum Level
        {
            Info, Warn, Error
        };

        public void Info(string msg)
        {
            Log(Level.Info, msg);
        }

        public void Warn(string msg)
        {
            Log(Level.Warn, msg);
        }

        public void Error(string msg)
        {
            Log(Level.Error, msg);
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
