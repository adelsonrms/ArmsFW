using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace ArmsFW.Infra.Identity
{
    public class EFDBContextProvider : ILoggerProvider, IDisposable
    {
        private readonly LoggerFactoryProviderConfiguration _config;

        private readonly ConcurrentDictionary<string, EFDBContextLogger> loggers = new ConcurrentDictionary<string, EFDBContextLogger>();

        public EFDBContextProvider(LoggerFactoryProviderConfiguration config)
        {
            _config = config;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, (string name) => new EFDBContextLogger(name, _config));
        }

    }
}