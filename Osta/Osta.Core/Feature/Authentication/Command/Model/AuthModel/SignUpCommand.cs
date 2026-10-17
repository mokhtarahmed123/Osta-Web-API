using MediatR;
using Microsoft.AspNetCore.Http;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authentication.Command.Model.AuthModel
{
    public record SignUpCommand(string FullName, string Email, string Password,
        string Phone, string ConfirmPassword, string Area, string City,
        string Governorate, DateOnly DateOfBirth, string Street, IFormFile? ProfileImage) :
        IRequest<Response<string>>;
}
