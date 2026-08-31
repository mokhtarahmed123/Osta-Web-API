namespace Osta.Identity.DTOs
{
    public enum SendResetPasswordCodeResult
    {
        Success,
        InvalidInput,
        UserNotFound,
        ErrorInUpdating,
        FailedToSendEmail,
        Failed
    }
}
