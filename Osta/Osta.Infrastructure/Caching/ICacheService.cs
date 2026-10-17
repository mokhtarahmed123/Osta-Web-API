namespace Osta.Infrastructure.Caching
{
    public interface ICacheService
    {
        Task<T?> GetDataAsync<T>(string key);
        Task SetDataAsync<T>(string key, T value, TimeSpan? time = null);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);

    }
}
