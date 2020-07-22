using SpecDrill.Infrastructure.Logging.Implementation;
using SpecDrill.Infrastructure.Logging.Interfaces;
using System;
using ILoggerFactory = SpecDrill.Infrastructure.Logging.Interfaces.ILoggerFactory;

namespace SpecDrill.Infrastructure.Logging
{
    public static class Log
    {
        private static readonly ILoggerFactory loggerFactory;

        static Log()
        {
            loggerFactory = new Log4NetFactory();
        }

        public static ILogger Get<T>()
            => loggerFactory.Get(typeof(T));

        public static ILogger Get(Type type)
            => loggerFactory.Get(type);
    }
}
