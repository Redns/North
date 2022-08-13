using North.Common;

namespace North.Services.Logger
{
    public interface ILogger
    {
        void Trace(string message);
        void Trace(string message, Exception e);
        void Debug(string message);
        void Debug(string message, Exception e);
        void Info(string message);
        void Info(string message, Exception e);
        void Warn(string message);
        void Warn(string message, Exception e);
        void Error(string message);
        void Error(string message, Exception e);
        void Fatal(string message);
        void Fatal(string message, Exception e);
        void ConfigLoggers(LogSetting? settings);
    }
}
