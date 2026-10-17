namespace Osta.Identity.DTOs
{
    public enum ConfirmResetPasswordResult
    {
        Success,
        UserNotFound,
        CodeIsWrong,

        InvalidInput,
        ErrorInUpdating,
        FailedToSendEmail
    }
}
