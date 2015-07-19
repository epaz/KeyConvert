using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using KeyConvert.Utils;

namespace KeyConvert.FrontendWpf
{
    public class WpfConsoleLogger : ILogger
    {
        private readonly TextBlock _logTextBlock;
        private readonly FileLogger _fileLogger;

        public WpfConsoleLogger(ref TextBlock logTextBlock)
        {
            if(logTextBlock == null)
                throw new ArgumentNullException("logTextBlock");
         
            _logTextBlock = logTextBlock;
            _fileLogger = new FileLogger();
        }

        public void Log(LogLevel level, string msg)
        {
            switch (level)
            {
                case LogLevel.Error:
                    _logTextBlock.Inlines.Add(new Run(msg) {Foreground = Brushes.Red});
                    break;
                case LogLevel.Warn:
                    _logTextBlock.Inlines.Add(new Run(msg) {Foreground = Brushes.DarkOrange});
                    break;
                case LogLevel.Info:
                    _logTextBlock.Inlines.Add(msg);
                    break;
            }
            
            _logTextBlock.Inlines.Add(new LineBreak());

            _fileLogger.Log(level, msg);
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

        public string GetFullLogText()
        {
            return _logTextBlock.Text;
        }

        public void SaveToFile(string filePath)
        {
            _fileLogger.SaveToFile(filePath);
        }
    }
}
