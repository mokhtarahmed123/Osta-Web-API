using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Osta.Core.Bases;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Osta.Core.HandlerMiddleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {

                _logger.LogInformation("Request was cancelled by the client.");
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning(ex, "Response already started, cannot write error response.");
                    throw;
                }

                _logger.LogError(ex, "Unhandled exception occurred.");

                var response = context.Response;
                response.ContentType = "application/json";

                var responseModel = new Response<string>
                {
                    Succeeded = false,
                    Message = "An unexpected error occurred.",
                    StatusCode = HttpStatusCode.InternalServerError
                };

                switch (ex)
                {
                    case UnauthorizedAccessException:
                        responseModel.StatusCode = HttpStatusCode.Unauthorized;
                        responseModel.Message = "You are not authorized to perform this action.";
                        break;

                    case ValidationException validationEx:
                        responseModel.StatusCode = HttpStatusCode.BadRequest;
                        responseModel.Message = validationEx.Message;
                        break;

                    case NotFoundException notFoundEx:
                        responseModel.StatusCode = HttpStatusCode.NotFound;
                        responseModel.Message = notFoundEx.Message;
                        break;

                    case KeyNotFoundException:
                        responseModel.StatusCode = HttpStatusCode.NotFound;
                        responseModel.Message = "The requested resource was not found.";
                        break;

                    case ForbiddenException:
                        responseModel.StatusCode = HttpStatusCode.Forbidden;
                        responseModel.Message = "Access to this resource is forbidden.";
                        break;

                    case DbUpdateException:
                        responseModel.StatusCode = HttpStatusCode.BadRequest;
                        responseModel.Message = "A database error occurred while processing your request.";
                        break;

                    default:
                        responseModel.StatusCode = HttpStatusCode.InternalServerError;
                        if (_env.IsDevelopment())
                        {
                            responseModel.Message = ex.Message;
                            if (ex.InnerException != null)
                                responseModel.Message += " | Inner: " + ex.InnerException.Message;
                        }
                        break;
                }

                response.StatusCode = (int)responseModel.StatusCode;

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var result = JsonSerializer.Serialize(responseModel, options);
                await response.WriteAsync(result);
            }
        }
    }
}