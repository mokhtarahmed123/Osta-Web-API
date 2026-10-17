using System.Net;

namespace Osta.Core.Bases
{
    public class ResponseHandler : IResponseHandler
    {

        public ResponseHandler()
        {

        }
        public Response<T> Success<T>(T? entity, object? meta = null) // For Retrieve operations, returns a successful response with the provided entity and optional metadata.
        {
            return new Response<T>
            {
                Data = entity,
                StatusCode = HttpStatusCode.OK,
                Succeeded = true,
                Message = "Success",
                Meta = meta
            };
        }

        public Response<T> Created<T>(T? entity, object? meta = null) // For Create operations, returns a successful response with the provided entity and optional metadata.
        {
            return new Response<T>
            {
                Data = entity,
                StatusCode = HttpStatusCode.Created,
                Succeeded = true,
                Message = " created successfully.",
                Meta = meta
            };
        }

        public Response<T> Updated<T>(string? message = null) // For Update operations, returns a successful response with an optional message.
        {
            return new Response<T>
            {
                StatusCode = HttpStatusCode.OK,
                Succeeded = true,
                Message = message ?? "Updated"
            };
        }

        public Response<T> Deleted<T>(string? message = null) // For Delete operations, returns a successful response with an optional message.
        {
            return new Response<T>
            {
                StatusCode = HttpStatusCode.OK,
                Succeeded = true,
                Message = message ?? "Deleted"
            };
        }

        public Response<T> BadRequest<T>(string? message = null) // For Bad Request responses, returns a response indicating a bad request with an optional message.
        {
            return new Response<T>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Succeeded = false,
                Message = message ?? "Bad Request"
            };
        }

        public Response<T> Unauthorized<T>(string? message = null) // For Unauthorized responses, returns a response indicating unauthorized access with an optional message.
        {
            return new Response<T>
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Succeeded = false,
                Message = message ?? "Unauthorized"
            };
        }

        public Response<T> Forbidden<T>(string? message = null) // For Forbidden responses, returns a response indicating forbidden access with an optional message.
        {
            return new Response<T>
            {
                StatusCode = HttpStatusCode.Forbidden,
                Succeeded = false,
                Message = message ?? "Forbidden"
            };
        }

        public Response<T> NotFound<T>(string? message = null)
        {
            return new Response<T>
            {
                StatusCode = HttpStatusCode.NotFound,
                Succeeded = false,
                Message = message ?? "Not Found"
            };
        }

        public Response<T> ServerError<T>(string? message = null)
        {
            return new Response<T>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Succeeded = false,
                Message = message ?? "Internal Server Error"
            };
        }

        public Response<T> Conflict<T>(string? message = null)
        {
            return new Response<T>
            {
                Succeeded = false,
                StatusCode = HttpStatusCode.Conflict,
                Message = message ?? "A conflict occurred."
            };
        }
    }
}


