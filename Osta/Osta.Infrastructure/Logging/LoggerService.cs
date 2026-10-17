using Microsoft.Extensions.Logging;
using Osta.SharedKernel.Logging;

namespace Osta.Infrastructure.Logging
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger<LoggerService> _logger;

        public LoggerService(ILogger<LoggerService> logger)
        {
            _logger = logger;
        }
        public void LogInformation(string message, params object[] args)
       => _logger.LogInformation(message, args);

        public void LogWarning(string message, params object[] args)
            => _logger.LogWarning(message, args);

        public void LogError(string message, params object[] args)
            => _logger.LogError(message, args);

        public void LogError(Exception ex, string message, params object[] args)
            => _logger.LogError(ex, message, args);

        public void LogCritical(Exception ex, string message, params object[] args)
            => _logger.LogCritical(ex, message, args);
    }
}
