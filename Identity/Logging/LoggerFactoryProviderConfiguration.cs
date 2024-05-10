using Microsoft.Extensions.Logging;

namespace ArmsFW.Infra.Identity
{
    public class LoggerFactoryProviderConfiguration
    {
        public LogLevel LogLevel { get; set; }

        public int EventId { get; set; }
    }
}