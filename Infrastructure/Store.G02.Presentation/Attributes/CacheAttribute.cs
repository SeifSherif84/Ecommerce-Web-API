using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Store.G02.Services.Abstractions;
using System.Text;

namespace Store.G02.Presentation.Attributes
{
    public class CacheAttribute(int timeInSec) : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var _serviceManager = context.HttpContext.RequestServices.GetRequiredService<IServiceManager>();
            var CacheKey = GenerateCacheKey(context.HttpContext.Request);
            var value = await _serviceManager.cacheService.GetAsync(CacheKey);
            if (!string.IsNullOrEmpty(value))
            {
                var ResponseBody = new ContentResult()
                {
                    Content = value,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = ResponseBody;
                return;
            }

            var actionContext = await next.Invoke();

            if(actionContext.Result is OkObjectResult okObjectResult)
                await _serviceManager.cacheService.SetAsync(CacheKey, okObjectResult.Value, TimeSpan.FromMinutes(timeInSec));            
        }

        private string GenerateCacheKey(HttpRequest request)
        {
            var CacheKey = new StringBuilder("");
            CacheKey.Append(request.Path);
            foreach (var queryParam in request.Query)
                CacheKey.Append(queryParam);
            return CacheKey.ToString();
        }

    }
}
