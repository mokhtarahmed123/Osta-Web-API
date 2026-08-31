using Microsoft.AspNetCore.Http;
using Osta.Data.Entities.Identity;
using Osta.Data.Helper;
using Osta.Identity.DTOs;

namespace Osta.Identity.Authentication
{
    public interface IAuthenticationService
    {
        public Task<JWTAuthResponse> GenerateJWToken(User user);

        public Task SaveRefreshToken(RefreshToken refreshToken, User user);
        public Task<JWTAuthResponse> GetRefreshToken(string refreshTokenString, string Token);
        Task RevokeRefreshToken(string UserId);
        public Task<SignUpResult> SignUpAsync(User user, string Password, IFormFile? ProfileImage);
        public Task<ConfirmEmailResult> ConfirmEmail(string UserId, string Code);
        public Task<SendResetPasswordCodeResult> SendResetPasswordCode(string email);
        public Task<ResetPasswordResult> ResetPasswordCode(string email, string Password);
        public Task<ConfirmResetPasswordResult> ConfirmResetPassword(string Code, string Email);
        public Task<string> ValidateToken(string Token);
    }
}
