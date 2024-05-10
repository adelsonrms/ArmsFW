using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace ArmsFW.Infra.Identity
{
    public class EFDBContextLogger : ILogger
    {
        private readonly LoggerFactoryProviderConfiguration _config;

        private readonly string _logerName;

        public EFDBContextLogger(string name, LoggerFactoryProviderConfiguration config)
        {
            _logerName = name;
            _config = config;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            RegistrarLogTexto($"{logLevel.ToString()}: Event {eventId.Id} ({eventId.Name}) - {formatter(state, exception)}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        private void RegistrarLogTexto(string logMessage)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(GetLogFileLocal(), append: true);
                streamWriter.WriteLineAsync(DateTime.Now.ToString() + " - EF Identity Core Log - " + logMessage);
                streamWriter.Close();
            }
            catch
            {
            }
        }

        private static string GetLogFileLocal()
        {
            string result = string.Empty;
            string location = Current.Instance().GetType().Assembly.Location;
            string text = new FileInfo(location).Directory.FullName + "\\_logs";
            Directory.CreateDirectory(text);
            if (File.Exists(location))
            {
                result = Path.Combine(text, "log.txt");
            }
            return result;
        }
    }
}