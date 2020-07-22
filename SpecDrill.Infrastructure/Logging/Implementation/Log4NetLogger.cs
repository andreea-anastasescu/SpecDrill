using log4net;
using SpecDrill.Infrastructure.Logging.Interfaces;
using System;
using System.Diagnostics;

namespace SpecDrill.Infrastructure.Logging.Implementation
{
    class Log4NetLogger : ILogger
    {
        private readonly ILog logger;

        public Log4NetLogger(ILog logger)
        {
            this.logger = logger;
        }

        public void Log(LogLevel level, object message)
        {
            CallLogger(level, message);
        }

        public void Log(LogLevel level, string format, params object[] args)
        {
            CallLogger(level, string.Format(format, args));
        }

        public void Log(LogLevel level, Exception exception, object message)
        {
            CallLogger(level, message, exception);
        }

        public void Log(LogLevel level, Exception exception, string format, params object[] args)
        {
            CallLogger(level, string.Format(format, args), exception);
        }

        private void CallLogger(LogLevel level, object message, Exception? exception = null)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    Trace.WriteLine($"DEBUG - {message}, {exception?.ToString() ?? "(null)"}");
                    if (logger.IsDebugEnabled)
                    {
                        logger.Debug(message, exception);
                    }
                    return;
                case LogLevel.Error:
                    Trace.WriteLine($"ERROR - {message}, {exception?.ToString() ?? "(null)"}");
                    if (logger.IsErrorEnabled)
                    {
                        logger.Error(message, exception);
                    }
                    return;
                case LogLevel.Fatal:
                    Trace.WriteLine($"FATAL - {message}, {exception?.ToString() ?? "(null)"}");
                    if (logger.IsFatalEnabled)
                    {
                        logger.Fatal(message, exception);
                    }
                    return;
                case LogLevel.Info:
                    Trace.WriteLine($"INFO - {message}, {exception?.ToString() ?? "(null)"}");
                    if (logger.IsInfoEnabled)
                    {
                        logger.Info(message, exception);
                    }
                    return;
                default:
                    Trace.WriteLine($"WARN - {message}, {exception?.ToString() ?? "(null)"}");
                    if (logger.IsWarnEnabled)
                    {
                        logger.Warn(message, exception);
                    }
                    return;
            }
        }
    }
}
