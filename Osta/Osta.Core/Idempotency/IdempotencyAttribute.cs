using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Osta.Core.Idempotency
{
    public class IdempotencyAttribute : ActionFilterAttribute
    {
        private const string IdempotencyKeyHeader = "X-Idempotency-Key";
        private const string InProgressStatus = "In-Progress";
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(IdempotencyKeyHeader, out var idempotencyKey) || string.IsNullOrWhiteSpace(idempotencyKey))
            {
                context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult($"Missing {IdempotencyKeyHeader} header.");
                return;
            }
            var cacheKey = $"Idempotency_{idempotencyKey}";
            var cacheService = context.HttpContext.RequestServices.GetService<IDistributedCache>();
            var cachedResponse = await cacheService.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedResponse))
            {
                if (cachedResponse == InProgressStatus)
                {
                    context.Result = new Microsoft.AspNetCore.Mvc.StatusCodeResult(409); // Conflict
                    return;
                }
                var idempotencyResult = JsonSerializer.Deserialize<IdempotencyResult>(cachedResponse);
                if (idempotencyResult != null)
                {
                    context.Result = new ObjectResult(idempotencyResult.Result)
                    {
                        StatusCode = idempotencyResult.StatusCode
                    };
                    return;
                }
            }
            else
            {
                await cacheService.SetStringAsync(cacheKey, InProgressStatus, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                }
                );
                var executedContext = await next();
                if (executedContext.Result is ObjectResult objectResult)
                {
                    if (objectResult.StatusCode > 200 && objectResult.StatusCode < 300)
                    {
                        var idempotencyResult = new IdempotencyResult
                        {
                            StatusCode = objectResult.StatusCode ?? 200,
                            Result = objectResult.Value
                        };
                        var serializedResponse = JsonSerializer.Serialize(idempotencyResult);
                        await cacheService.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                        });
                    }
                }
                else
                {
                    await cacheService.RemoveAsync(cacheKey);
                }
            }


        }
    }
    public class IdempotencyResult
    {
        public int StatusCode { get; set; }
        public object? Result { get; set; }
    }
}
