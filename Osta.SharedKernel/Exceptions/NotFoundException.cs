namespace Osta.Core.HandlerMiddleware
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }

    }
}
