using North.Common;
using System.Text;

namespace North.Services.Logger
{
    public class KLogger : ILogger, IDisposable
    {
        private readonly FileStream logger;
        private readonly LogSetting setting;

        public KLogger(LogSetting setting)
        {
            this.setting = setting;
            if (!File.Exists(setting.Output))
            {
                var outputDir = Path.GetDirectoryName(setting.Output);
                if (!string.IsNullOrEmpty(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                logger = new FileStream(setting.Output, FileMode.Create);
            }
            else
            {
                logger = new FileStream(setting.Output, FileMode.Append);
            }
        }

        public void ConfigLoggers(LogSetting settings)
        {
            
        }

        public void Debug(string message)
        {
            if((LogLevel.Debug >= setting.Level.Min) && (LogLevel.Debug <= setting.Level.Max))
            {
                var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [North Debug] {message}\n";

                Console.Write(msg);
                logger.Write(Encoding.UTF8.GetBytes(msg));
            }
        }

        public void Debug(string message, Exception e)
        {
            if ((LogLevel.Debug >= setting.Level.Min) && (LogLevel.Debug <= setting.Level.Max))
            {
                var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [North Debug] {message}, {e.Message}\n{e.StackTrace}";

                Console.Write(msg);
                logger.Write(Encoding.UTF8.GetBytes(msg));
            }
        }


        public void Error(string message)
        {
            if ((LogLevel.Error >= setting.Level.Min) && (LogLevel.Error <= setting.Level.Max))
            {
                var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [North Error] {message}\n";

                Console.Write(msg);
                logger.Write(Encoding.UTF8.GetBytes(msg));
            }
        }


        public void Error(string message, Exception e)
        {
            if ((LogLevel.Error >= setting.Level.Min) && (LogLevel.Error <= setting.Level.Max))
            {
                var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [North Error] {message}, {e.Message}\n{e.StackTrace}";

                Console.Write(msg);
                logger.Write(Encoding.UTF8.GetBytes(msg));
            }
        }


        public void Fatal(string message)
        {
            if ((LogLevel.Critical >= setting.Level.Min) && (LogLevel.Critical <= setting.Level.Max))
            {
                var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [North Fatal] {message}\n";

                Console.Write(msg);
                logger.Write(Encoding.UTF8.GetBytes(msg));
            }
        }


        public void Fatal(string message, Exception e)
        {
            if ((LogLevel.Critical >= setting.Level.Min) && (LogLevel.Critical <= setting.Level.Max))
            {
                var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [North Fatal] {message}, {e.Message}\n{e.StackTrace}";

                Console.Write(msg);
                logger.Write(Encoding.UTF8.GetBytes(msg));
            }
        }


        public void Info(string message)
        {
            if ((LogLevel.Information >= setting.Level.Min) && (LogLevel.Information <= setting.Level.Max))
            {
                var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [North Info] {message}\n";

                Console.Write(msg);
                logger.Write(Encoding.UTF8.GetBytes(msg));
            }
        }


        public void Info(string message, Exception e)
        {
            if ((LogLevel.Information >= setting.Level.Min) && (LogLevel.Information <= setting.Level.Max))
            {
                var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [North Info] {message}, {e.Message}\n{e.StackTrace}";

                Console.Write(msg);
                logger.Write(Encoding.UTF8.GetBytes(msg));
            }
        }


        public void Trace(string message)
        {
            if ((LogLevel.Trace >= setting.Level.Min) && (LogLevel.Trace <= setting.Level.Max))
            {
                var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [North Trace] {message}\n";

                Console.Write(msg);
                logger.Write(Encoding.UTF8.GetBytes(msg));
            }
        }


        public void Trace(string message, Exception e)
        {
            if ((LogLevel.Trace >= setting.Level.Min) && (LogLevel.Trace <= setting.Level.Max))
            {
                var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [North Trace] {message}, {e.Message}\n{e.StackTrace}";

                Console.Write(msg);
                logger.Write(Encoding.UTF8.GetBytes(msg));
            }
        }


        public void Warn(string message)
        {
            if ((LogLevel.Warning >= setting.Level.Min) && (LogLevel.Warning <= setting.Level.Max))
            {
                var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [North Warn] {message}\n";

                Console.Write(msg);
                logger.Write(Encoding.UTF8.GetBytes(msg));
            }
        }


        public void Warn(string message, Exception e)
        {
            if ((LogLevel.Warning >= setting.Level.Min) && (LogLevel.Warning <= setting.Level.Max))
            {
                var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [North Warn] {message}, {e.Message}\n{e.StackTrace}";

                Console.Write(msg);
                logger.Write(Encoding.UTF8.GetBytes(msg));
            }
        }


        public void Dispose()
        {
            logger.Flush();
            logger.Close();
            GC.SuppressFinalize(this);
        }
    }
}
