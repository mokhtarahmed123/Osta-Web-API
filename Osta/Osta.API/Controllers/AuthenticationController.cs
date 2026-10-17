using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Core.Feature.Authentication.Query.Model;
using Osta.Core.Feature.Authentication.Query.Model.AuthModel;
using Osta.Core.Feature.Emails.Command.Model;
using Osta.Core.Feature.Emails.Query.Model;
using Osta.Data.Helper;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : AppBaseController
    {
        [HttpPost("SignUp")]
        [SwaggerOperation(Summary = "Creates a new user account", Description = "Registers a new user account using the provided information.")]
        [SwaggerResponse(200, "User registered successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid registration data")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> SignUp([FromQuery] SignUpCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }




        [HttpPost("LogIn")]
        [SwaggerOperation(Summary = "Logs in a user", Description = "Authenticates a user using the provided login credentials and returns authentication information.")]
        [SwaggerResponse(200, "User logged in successfully", type: typeof(JWTAuthResponse))]
        [SwaggerResponse(400, "Invalid login data")]
        [SwaggerResponse(401, "Invalid email or password")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [Authorize]
        [HttpPost("Logout")]

        [SwaggerOperation(Summary = "Logs out the current user", Description = "Logs out the currently authenticated user and invalidates the current authentication session.")]
        [SwaggerResponse(200, "User logged out successfully", type: typeof(string))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Logout()
        {
            var response = await Mediator.Send(new LogOutCommand());
            return NewResult(response);

        }

        [HttpPost("SendEmail")]

        [SwaggerOperation(Summary = "Sends a confirmation email", Description = "Sends an email to the user for email confirmation.")]
        [SwaggerResponse(200, "Email sent successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid email data")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> SendEmail([FromQuery] SendEmailCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }


        [HttpGet("ConfirmEmail")]
        [SwaggerOperation(Summary = "Sends a confirmation email", Description = "Sends an email to the user for email confirmation.")]
        [SwaggerResponse(200, "Email sent successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid email data")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailQuery command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }


        [HttpPost("SendResetPassword")]
        [SwaggerOperation(Summary = "Sends a password reset email", Description = "Sends a password reset link to the user's email address.")]
        [SwaggerResponse(200, "Password reset email sent successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid email data")]
        [SwaggerResponse(404, "User not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> SendResetPassword([FromQuery] SendResetPasswordCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);

        }

        [HttpGet("ConfirmResetPassword")]
        [SwaggerOperation(Summary = "Confirms password reset request", Description = "Validates the password reset confirmation information.")]
        [SwaggerResponse(200, "Password reset request confirmed successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid reset password data")]
        [SwaggerResponse(404, "User not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> ConfirmResetPassword([FromQuery] ConfirmResetPasswordQuery command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);

        }


        [HttpPost("ResetPassword")]

        [SwaggerOperation(Summary = "Resets the user's password", Description = "Resets the user's password using the provided reset password information.")]
        [SwaggerResponse(200, "Password reset successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid reset password data")]
        [SwaggerResponse(404, "User not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> ResetPassword([FromQuery] ResetPasswordCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPost("RefreshToken")]
        [SwaggerOperation(Summary = "Refreshes the authentication token", Description = "Generates a new access token using a valid refresh token.")]
        [SwaggerResponse(200, "Token refreshed successfully", type: typeof(JWTAuthResponse))]
        [SwaggerResponse(400, "Invalid refresh token")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> RefreshToken([FromQuery] RefreshTokenCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);


        }

        [Authorize]
        [HttpGet("MyProfile")]
        [SwaggerOperation(Summary = "Gets the current user's profile", Description = "Retrieves the profile information of the currently authenticated user.")]
        [SwaggerResponse(200, "User profile retrieved successfully", type: typeof(MyProfileQueryResult))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "User profile not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> MyProfile()
        {
            var response = await Mediator.Send(new MyProfileQuery());
            return NewResult(response);
        }

        [HttpPost("google-login")]
        [SwaggerOperation(Summary = "Logs in using Google", Description = "Authenticates a user using a Google ID token.")]
        [SwaggerResponse(200, "Google login successful", type: typeof(JWTAuthResponse))]
        [SwaggerResponse(400, "Invalid Google token")]
        [SwaggerResponse(401, "Google authentication failed")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginDto dto)
        {
            var result = await Mediator.Send(new GoogleLoginCommand(dto.IdToken));
            return NewResult(result);
        }
        //[HttpPost("facebook-login")]
        //public async Task<IActionResult> FacebookLogin(FacebookLoginCommand command)
        //{
        //    var result = await Mediator.Send(command);
        //    return NewResult(result);
        //}
    }
}
