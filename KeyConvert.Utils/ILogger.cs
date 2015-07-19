namespace KeyConvert.Utils
{
    public interface ILogger
    {
        void Log(LogLevel level, string msg);
        void Info(string msg);
        void Warn(string msg);
        void Error(string msg);
        string GetFullLogText();
        void SaveToFile(string filePath);
    }
}