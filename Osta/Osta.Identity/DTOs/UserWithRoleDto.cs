namespace Osta.Data.Helper
{
    public record UserWithRoleDto(string Id, string UserName, string Email, bool IsActive, string RoleName);
}
