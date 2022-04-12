using NLog;

namespace ImageBed.Common
{
    /// <summary>
    /// nLog使用帮助类
    /// </summary>
    public class LoggerHelper
    {
        /// <summary>
        /// 实例化nLog，即为获取配置文件相关信息(获取以当前正在初始化的类命名的记录器)
        /// </summary>
        private readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private static LoggerHelper? _obj;

        public static LoggerHelper _
        {
            get => _obj ?? (new LoggerHelper());
            set => _obj = value;
        }

        #region Debug，调试
        public void Debug(string msg)
        {
            _logger.Debug(msg);
        }

        public void Debug(string msg, Exception err)
        {
            _logger.Debug(err, msg);
        }
        #endregion

        #region Info，信息
        public void Info(string msg)
        {
            _logger.Info(msg);
        }

        public void Info(string msg, Exception err)
        {
            _logger.Info(err, msg);
        }
        #endregion

        #region Warn，警告
        public void Warn(string msg)
        {
            _logger.Warn(msg);
        }

        public void Warn(string msg, Exception err)
        {
            _logger.Warn(err, msg);
        }
        #endregion

        #region Trace，追踪
        public void Trace(string msg)
        {
            _logger.Trace(msg);
        }

        public void Trace(string msg, Exception err)
        {
            _logger.Trace(err, msg);
        }
        #endregion

        #region Error，错误
        public void Error(string msg)
        {
            _logger.Error(msg);
        }

        public void Error(string msg, Exception err)
        {
            _logger.Error(err, msg);
        }
        #endregion

        #region Fatal,致命错误
        public void Fatal(string msg)
        {
            _logger.Fatal(msg);
        }

        public void Fatal(string msg, Exception err)
        {
            _logger.Fatal(err, msg);
        }
        #endregion
    }
}
