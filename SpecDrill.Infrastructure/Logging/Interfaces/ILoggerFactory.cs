using System;
namespace SpecDrill.Infrastructure.Logging.Interfaces
{
    public interface ILoggerFactory
    {
        ILogger Get(Type name);
    }
}
