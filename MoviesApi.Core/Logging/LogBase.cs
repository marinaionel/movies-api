using log4net;
using log4net.Core;
using System;
using System.Reflection;

namespace movies_api.Core.Logging
{
    public abstract class LogBase<T> where T : LogBase<T>, new()
    {
        private readonly ILog _logger;

        public bool IsDebugEnabled => _logger.IsDebugEnabled;
        public bool IsTraceEnabled => _logger.Logger.IsEnabledFor(Level.Trace);

        private static T _default;
        public static T Default => _default ??= new T();

        protected LogBase(string logTag)
        {
            _logger = LogManager.GetLogger(logTag);
        }

        public void Info(string text) => _logger.Info(text);
        public void Debug(string text) => _logger.Debug(text);
        public void Warn(string text) => _logger.Warn(text);
        public void Trace(string text) => _logger.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType, Level.Trace, text, null);

        public void Error(string text, Exception exception = null)
        {
            _logger.Error(text);

            if (exception != null)
                _logger.Error(exception);
        }

        public void Fatal(string text, Exception exception = null)
        {
            _logger.Fatal(text);

            if (exception != null)
                _logger.Fatal(exception);
        }
    }
}
