namespace Osta.Core.Bases
{
    public interface IResponseHandler
    {
        Response<T> Success<T>(T? entity, object? meta = null);
        Response<T> Created<T>(T entity, object? meta = null);
        Response<T> Updated<T>(string? message = null);
        Response<T> Deleted<T>(string? message = null);
        Response<T> BadRequest<T>(string? message = null);
        Response<T> Unauthorized<T>(string? message = null);
        Response<T> Forbidden<T>(string? message = null);
        Response<T> NotFound<T>(string? message = null);
        Response<T> ServerError<T>(string? message = null);

        Response<T> Conflict<T>(string? message = null);
    }
}
