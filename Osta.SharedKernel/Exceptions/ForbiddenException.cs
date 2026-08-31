namespace Osta.Core.HandlerMiddleware
{
    public class ForbiddenException : Exception
    {

        public ForbiddenException(string message) : base(message)
        {
        }
    }
}
