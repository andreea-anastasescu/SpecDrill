using SpecDrill.Infrastructure.Logging.Interfaces;
using System;
using System.Diagnostics;

namespace SpecDrill.Infrastructure.Logging
{
    public static class LoggerExtensions
    {
        public static void Info(this ILogger logger, object message)
        {
            if (logger == null)
                Trace.Write(message);
            else
                logger.Log(LogLevel.Info, message);
        }
        public static void Info(this ILogger logger, string format, params object[] args)
        {
            if (logger == null)
                Trace.Write(string.Format(format, args));
            else
                logger.Log(LogLevel.Info, format, args);

        }
        public static void Info(this ILogger logger, Exception exception, object message)
        {
            if (logger == null)
                Trace.Write($"{exception.Message}:{message}");
            else
                logger.Log(LogLevel.Info, exception, message);
        }
        public static void Info(this ILogger logger, Exception exception, string format, params object[] args)
        {
            if (logger == null)
                Trace.Write($"{exception.Message}:{string.Format(format, args)}");
            else
                logger.Log(LogLevel.Info, exception, format, args);
        }

        public static void Warning(this ILogger logger, object message)
        {
            if (logger == null)
                Trace.Write(message);
            else
                logger.Log(LogLevel.Warning, message);
        }
        public static void Warning(this ILogger logger, string format, params object[] args)
        {
            if (logger == null)
                Trace.Write(string.Format(format, args));
            else
                logger.Log(LogLevel.Warning, format, args);
        }
        public static void Warning(this ILogger logger, Exception exception, object message)
        {
            if (logger == null)
                Trace.Write($"{exception.Message}:{message}");
            else
                logger.Log(LogLevel.Warning, exception, message);
        }
        public static void Warning(this ILogger logger, Exception exception, string format, params object[] args)
        {
            if (logger == null)
                Trace.Write($"{exception.Message}:{string.Format(format, args)}");
            else
                logger.Log(LogLevel.Warning, exception, format, args);
        }

        public static void Debug(this ILogger logger, object message)
        {
            if (logger == null)
                Trace.Write(message);
            else
                logger.Log(LogLevel.Debug, message);
        }
        public static void Debug(this ILogger logger, string format, params object[] args)
        {
            if (logger == null)
                Trace.Write(string.Format(format, args));
            else
                logger.Log(LogLevel.Debug, format, args);
        }
        public static void Debug(this ILogger logger, Exception exception, object message)
        {
            if (logger == null)
                Trace.Write($"{exception.Message}:{message}");
            else
                logger.Log(LogLevel.Debug, exception, message);
        }
        public static void Debug(this ILogger logger, Exception exception, string format, params object[] args)
        {
            if (logger == null)
                Trace.Write($"{exception.Message}:{string.Format(format, args)}");
            else
                logger.Log(LogLevel.Debug, exception, format, args);
        }

        public static void Error(this ILogger logger, object message)
        {
            if (logger == null)
                Trace.Write(message);
            else
                logger.Log(LogLevel.Error, message);
        }
        public static void Error(this ILogger logger, string format, params object[] args)
        {
            if (logger == null)
                Trace.Write(string.Format(format, args));
            else
                logger.Log(LogLevel.Error, format, args);
        }
        public static void Error(this ILogger logger, Exception exception, object message)
        {
            if (logger == null)
                Trace.Write($"{exception.Message}:{message}");
            else
                logger.Log(LogLevel.Error, exception, message);
        }
        public static void Error(this ILogger logger, Exception exception, string format, params object[] args)
        {
            if (logger == null)
                Trace.Write($"{exception.Message}:{string.Format(format, args)}");
            else
                logger.Log(LogLevel.Error, exception, format, args);
        }

        public static void Fatal(this ILogger logger, object message)
        {
            if (logger == null)
                Trace.Write(message);
            else
                logger.Log(LogLevel.Fatal, message);
        }
        public static void Fatal(this ILogger logger, string format, params object[] args)
        {
            if (logger == null)
                Trace.Write(string.Format(format, args));
            else
                logger.Log(LogLevel.Fatal, format, args);
        }
        public static void Fatal(this ILogger logger, Exception exception, object message)
        {
            if (logger == null)
                Trace.Write($"{exception.Message}:{message}");
            else
                logger.Log(LogLevel.Fatal, exception, message);
        }
        public static void Fatal(this ILogger logger, Exception exception, string format, params object[] args)
        {
            if (logger == null)
                Trace.Write($"{exception.Message}:{string.Format(format, args)}");
            else
                logger.Log(LogLevel.Fatal, exception, format, args);
        }
    }
}
