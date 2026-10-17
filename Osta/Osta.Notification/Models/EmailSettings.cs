namespace Osta.Data.Helper
{
    public class EmailSettings
    {
        public string Username { get; set; } = string.Empty;

        public string FromEmail { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string SmtpServer { get; set; } = string.Empty;

        public int SmtpPort { get; set; }

        public string FromName { get; set; } = string.Empty;
    }
}
