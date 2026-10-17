namespace Osta.SharedKernel.Logging
{
    public interface ILoggerService
    {
        void LogInformation(string message, params object[] args);

        void LogWarning(string message, params object[] args);

        void LogError(string message, params object[] args);

        void LogError(Exception ex, string message, params object[] args);

        void LogCritical(Exception ex, string message, params object[] args);
    }
}
