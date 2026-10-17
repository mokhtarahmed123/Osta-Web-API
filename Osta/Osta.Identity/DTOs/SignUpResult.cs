namespace Osta.Identity.DTOs
{
    public enum SignUpResult
    {
        Success,
        InvalidInput,
        UserWithEmailAlreadyExists,
        UserCreationFailed,
        DefaultRoleNotFound,
        RoleAssignmentFailed,
        HttpContextNotAvailable,
        FailedToSendEmail,
        Failed
    }
}
