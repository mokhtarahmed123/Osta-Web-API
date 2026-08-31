using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Osta.Data.Entities.Identity;
using Osta.Data.Helper;
using Osta.Identity.Authorization;
using Osta.Identity.DTOs;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Notification.DTOs;
using Osta.Notification.Interfaces;
using Osta.SharedKernel;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Osta.Identity.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {

        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        private readonly ConcurrentDictionary<string, RefreshToken> useRefreshToken;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly OstaContext appDbContext;
        private readonly IAuthorizationService iAuthorizationService;
        private readonly IFileService imageUpload;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailService emailService;
        private readonly JWTModel _JWTSettings;

        public AuthenticationService(RoleManager<Role> roleManager,
            UserManager<User> userManager, ConcurrentDictionary<string, RefreshToken> UseRefreshToken,
            IHttpContextAccessor httpContextAccessor, IEmailService emailService, OstaContext appDbContext,
            IAuthorizationService IAuthorizationService, IFileService imageUpload, IOptions<JWTModel> emailSettings, IUnitOfWork unitOfWork)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
            useRefreshToken = UseRefreshToken;
            this.httpContextAccessor = httpContextAccessor;
            this.appDbContext = appDbContext;
            iAuthorizationService = IAuthorizationService;
            this.imageUpload = imageUpload;
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
            _JWTSettings = emailSettings.Value;

        }

        public async Task<ConfirmEmailResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(code))
            {
                return ConfirmEmailResult.UserIdOrCodeNull;
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return ConfirmEmailResult.UserNotFound;
            }
            user.IsActive = true;
            var result = await userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
            {
                return ConfirmEmailResult.Failed;
            }

            return ConfirmEmailResult.Confirmed;
        }
        public async Task<ConfirmResetPasswordResult> ConfirmResetPassword(
      string code,
      string email)
        {
            if (string.IsNullOrWhiteSpace(code) ||
                string.IsNullOrWhiteSpace(email))
            {
                return ConfirmResetPasswordResult.InvalidInput;
            }

            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return ConfirmResetPasswordResult.UserNotFound;
            }

            if (string.IsNullOrWhiteSpace(user.Code))
            {
                return ConfirmResetPasswordResult.CodeIsWrong;
            }

            if (!string.Equals(user.Code, code, StringComparison.Ordinal))
            {
                return ConfirmResetPasswordResult.CodeIsWrong;
            }

            return ConfirmResetPasswordResult.Success;
        }

        public async Task<JWTAuthResponse> GenerateJWToken(User user)
        {
            int RefreshTokenExpiredDate = (_JWTSettings.RefreshTokenExpiredDate);
            var Token = await GetJwtToken(user);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(Token);
            var RefreshToken = new RefreshToken
            {
                Created = DateTime.UtcNow,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(RefreshTokenExpiredDate),
                refreshToken = GenerateRefreshToken()
            };
            await SaveRefreshToken(RefreshToken, user);
            useRefreshToken.AddOrUpdate(RefreshToken.refreshToken, RefreshToken, (s, t) => RefreshToken);
            return new JWTAuthResponse
            {
                Token = tokenString,

                RefreshToken = new RefreshTokenResponse
                {
                    UserId = RefreshToken.UserId,
                    Created = RefreshToken.Created,
                    Expires = RefreshToken.Expires,
                    RefreshToken = RefreshToken.refreshToken
                }
            };

        }

        public async Task<JWTAuthResponse> GetRefreshToken(string refreshTokenString, string accessToken)
        {
            var token = ReadJWToken(accessToken);
            if (token is null || token.Header.Alg != SecurityAlgorithms.HmacSha256)
                throw new SecurityTokenException("Invalid token.");

            var userId = token.Claims
                .FirstOrDefault(x => x.Type == nameof(UserClaimModel.Id))?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new SecurityTokenException("Invalid token claims.");

            var user = await userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(x => x.Id == userId)
                ?? throw new KeyNotFoundException("User not found.");

            var refreshToken = user.RefreshTokens
                .FirstOrDefault(x => x.refreshToken == refreshTokenString)
                ?? throw new SecurityTokenException("Refresh token not found.");

            if (!refreshToken.IsActive)
                throw new SecurityTokenException(
                    refreshToken.Revoked != null
                        ? "Refresh token has been revoked."
                        : "Refresh token has expired."
                );

            refreshToken.Revoked = DateTime.UtcNow;
            await userManager.UpdateAsync(user);

            return await GenerateJWToken(user);
        }

        public async Task<ResetPasswordResult> ResetPasswordCode(
    string email,
    string password)
        {
            using var transaction =
                await unitOfWork.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(password))
                {
                    return ResetPasswordResult.InvalidInput;
                }

                var user = await userManager.FindByEmailAsync(email);

                if (user is null)
                {
                    return ResetPasswordResult.UserNotFound;
                }

                var removeResult = await userManager.RemovePasswordAsync(user);

                if (!removeResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return ResetPasswordResult.Failed;
                }

                var addResult = await userManager.AddPasswordAsync(user, password);

                if (!addResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return ResetPasswordResult.InvalidPassword;
                }

                await transaction.CommitAsync();

                return ResetPasswordResult.Success;
            }
            catch
            {
                await transaction.RollbackAsync();
                return ResetPasswordResult.Failed;
            }
        }

        public async Task RevokeRefreshToken(string UserId)
        {
            var user = await userManager.Users
               .Include(u => u.RefreshTokens)
               .FirstOrDefaultAsync(u => u.Id == UserId);

            if (user == null) return;

            foreach (var token in user.RefreshTokens.Where(t => t.Revoked == null && !t.IsExpired))
            {
                token.Revoked = DateTime.UtcNow;
                token.Expires = DateTime.UtcNow;
            }

            await userManager.UpdateAsync(user);
        }

        public async Task SaveRefreshToken(RefreshToken refreshToken, User user)
        {
            refreshToken.UserId = user.Id;
            appDbContext.RefreshTokens.Add(refreshToken);
            await appDbContext.SaveChangesAsync();

        }

        public async Task<SendResetPasswordCodeResult> SendResetPasswordCode(string email)
        {

            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return SendResetPasswordCodeResult.InvalidInput;
                }

                var user = await userManager.FindByEmailAsync(email);
                if (user == null) return SendResetPasswordCodeResult.UserNotFound;

                var codeBytes = new byte[4];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(codeBytes);
                }
                int codeInt = BitConverter.ToInt32(codeBytes, 0) & 0x7FFFFFFF;
                string code = (codeInt % 1000000).ToString("D6");


                user.Code = code;

                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded) return SendResetPasswordCodeResult.ErrorInUpdating;

                await transaction.CommitAsync();

                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    return SendResetPasswordCodeResult.InvalidInput;
                }

                var emailBody = $"<h3>Hello {user.UserName}!</h3>" +
                                $"<p>Your password reset code is: <strong>{user.Code}</strong></p>" +
                                $"<p>Please use this code to reset your password.</p>";

                var Emaildto = new Emaildto(Email: user.Email, emailBody, "Reset Password");

                var emailResult = await emailService.SendEmailAsync(Emaildto);

                if (emailResult != "Success")
                    return SendResetPasswordCodeResult.FailedToSendEmail;

                return SendResetPasswordCodeResult.Success;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return SendResetPasswordCodeResult.Failed;
            }
        }

        public async Task<SignUpResult> SignUpAsync(
            User user,
            string password,
            IFormFile? image)
        {
            await using var transaction =
                await unitOfWork.BeginTransactionAsync();

            try
            {
                if (user is null ||
                    string.IsNullOrWhiteSpace(user.Email) ||
                    string.IsNullOrWhiteSpace(password))
                {
                    return SignUpResult.InvalidInput;
                }

                // Check if email already exists
                var existingUser =
                    await userManager.FindByEmailAsync(user.Email);

                if (existingUser is not null)
                {
                    return SignUpResult.UserWithEmailAlreadyExists;
                }

                // Upload profile image
                if (image is not null)
                {
                    var ImagehttpContext = httpContextAccessor.HttpContext;

                    if (ImagehttpContext is null)
                    {
                        return SignUpResult.HttpContextNotAvailable;
                    }

                    var Imagerequest = ImagehttpContext.Request;

                    var baseUrl = $"{Imagerequest.Scheme}://{Imagerequest.Host}";
                    var location = $"Images/User/{user.Id}";

                    var imagePath =
                        await imageUpload.UploadImageAsync(image, location);

                    user.ProfileImage = baseUrl + imagePath;
                }

                // Create user
                var createResult =
                    await userManager.CreateAsync(user, password);

                if (!createResult.Succeeded)
                {
                    return SignUpResult.UserCreationFailed;
                }

                // Get default role
                var role =
                    await roleManager.FindByNameAsync("User");

                if (role is null)
                {
                    return SignUpResult.DefaultRoleNotFound;
                }

                // Assign role
                var addRoleResult =
                    await iAuthorizationService.AssignRoleToUserAsync(
                        role.Id,
                        user.Id);

                if (!addRoleResult.Succeeded)
                {
                    return SignUpResult.RoleAssignmentFailed;
                }

                // Generate email confirmation token
                var confirmationToken =
                    await userManager.GenerateEmailConfirmationTokenAsync(user);

                var encodedToken =
                    Uri.EscapeDataString(confirmationToken);

                // Generate confirmation link
                var httpContext =
                    httpContextAccessor.HttpContext;

                if (httpContext is null)
                {
                    return SignUpResult.HttpContextNotAvailable;
                }

                var request = httpContext.Request;

                var confirmationLink =
                    $"{request.Scheme}://{request.Host}" +
                    $"/api/Authentication/ConfirmEmail" +
                    $"?userId={user.Id}" +
                    $"&code={encodedToken}";

                // Prepare email
                var emailBody =
                    $"<h3>Welcome {user.UserName}!</h3>" +
                    $"<p>Please confirm your email by clicking the link below:</p>" +
                    $"<a href='{confirmationLink}'>Confirm Email</a>";

                var emailDto = new Emaildto(
                    user.Email,
                    emailBody,
                    "Confirm your email");

                // Send email
                var emailResult =
                    await emailService.SendEmailAsync(emailDto);

                if (emailResult != "Success")
                {
                    return SignUpResult.FailedToSendEmail;
                }

                await transaction.CommitAsync();

                return SignUpResult.Success;
            }
            catch
            {
                await transaction.RollbackAsync();

                return SignUpResult.Failed;
            }
        }
        public Task<string> ValidateToken(string Token)
        {
            var Handler = new JwtSecurityTokenHandler();

            var secretKey = _JWTSettings.SecretKey;
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _JWTSettings.IssuerIP,

                ValidateAudience = true,
                ValidAudience = _JWTSettings.AudienceIP,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                RoleClaimType = "roleName",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            var validtor = Handler.ValidateToken(Token, parameters, out SecurityToken validatedToken);
            try
            {
                if (validtor == null) throw new SecurityTokenException("InvalidToken");
                return Task.FromResult("Success");
            }
            catch (Exception ex)
            {
                return Task.FromResult(ex.Message);
            }
        }


        private JwtSecurityToken ReadJWToken(string JWToken)
        {
            if (string.IsNullOrEmpty(JWToken))
                throw new ArgumentNullException(nameof(JWToken));
            var Handler = new JwtSecurityTokenHandler();
            return Handler.ReadJwtToken(JWToken);
        }

        private string GenerateRefreshToken()
        {
            var random = new Byte[32];
            using var genrator = RandomNumberGenerator.Create();
            genrator.GetBytes(random);
            return Convert.ToBase64String(random);
        }




        private async Task<JwtSecurityToken> GetJwtToken(User user)
        {
            var roleName = await rolename(user);

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new InvalidOperationException("User email is not available.");
            }

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new InvalidOperationException("User Name is not available.");
            }



            var Claims = new List<Claim>
            {
                new Claim(nameof(UserClaimModel.Id),user.Id),
                new Claim(nameof(UserClaimModel.Email),user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(nameof(UserClaimModel.roleName),roleName),
                new Claim(nameof(UserClaimModel.UserName),user.UserName)
            };
            var secretKey = _JWTSettings.SecretKey;
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var Sign = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            int AccessTokenExpiredDate = (_JWTSettings.AccessTokenExpiredDate);
            int RefreshTokenExpiredDate = (_JWTSettings.RefreshTokenExpiredDate);

            var Token = new JwtSecurityToken
                (
                issuer: _JWTSettings.IssuerIP,
                audience: _JWTSettings.AudienceIP,
                claims: Claims,
                expires: DateTime.UtcNow.AddDays(AccessTokenExpiredDate),
                signingCredentials: Sign

                );

            return Token;
        }
        private async Task<string> rolename(User user)
        {
            return (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;
        }

    }
}
