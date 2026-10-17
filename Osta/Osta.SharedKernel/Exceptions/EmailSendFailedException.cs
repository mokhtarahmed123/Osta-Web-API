namespace Osta.SharedKernel.Exceptions
{
    public class EmailSendFailedException : Exception
    {
        public EmailSendFailedException()
      : base("Failed to send email.")
        {
        }

        public EmailSendFailedException(string message)
            : base(message)
        {
        }

    }
}
