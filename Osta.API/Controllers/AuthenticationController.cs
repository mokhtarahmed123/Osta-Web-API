using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Core.Feature.Authentication.Query.Model;
using Osta.Core.Feature.Authentication.Query.Model.AuthModel;
using Osta.Core.Feature.Emails.Command.Model;
using Osta.Core.Feature.Emails.Query.Model;

namespace Osta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : AppBaseController
    {
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromQuery] SignUpCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpPost("LogIn")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var response = await Mediator.Send(new LogOutCommand());
            return NewResult(response);

        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail([FromQuery] SendEmailCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }


        [HttpGet("ConfirmEmail")]

        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailQuery command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }


        [HttpPost("SendResetPassword")]
        public async Task<IActionResult> SendResetPassword([FromQuery] SendResetPasswordCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);

        }

        [HttpGet("ConfirmResetPassword")]
        public async Task<IActionResult> ConfirmResetPassword([FromQuery] ConfirmResetPasswordQuery command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);

        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromQuery] ResetPasswordCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);


        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromQuery] RefreshTokenCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);


        }

        [Authorize]
        [HttpGet("MyProfile")]
        public async Task<IActionResult> MyProfile()
        {
            var response = await Mediator.Send(new MyProfileQuery());
            return NewResult(response);
        }

        [HttpPost("google-login")]
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
