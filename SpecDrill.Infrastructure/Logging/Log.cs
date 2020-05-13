using System;
using SpecDrill.Infrastructure.Logging.Implementation;
using SpecDrill.Infrastructure.Logging.Interfaces;
using ILoggerFactory = SpecDrill.Infrastructure.Logging.Interfaces.ILoggerFactory;
using System.Diagnostics;

namespace SpecDrill.Infrastructure.Logging
{
    public static class Log
    {
        private static readonly ILoggerFactory loggerFactory;

        static  Log()
        {
            loggerFactory = new Log4NetFactory();
        }

        public static ILogger Get<T>()
        {
            return GetLogger(loggerFactory.Get(typeof(T)));
        }

        public static ILogger Get(Type type)
        {
            return GetLogger(loggerFactory.Get(type));
        }
        private static ILogger GetLogger(ILogger logger)
        {
            if (logger == null)
                Trace.Write($"Logger is null!");
            return logger;
        }

    }
}
