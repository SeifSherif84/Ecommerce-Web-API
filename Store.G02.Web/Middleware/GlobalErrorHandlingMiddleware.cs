using Store.G02.Domain.Exceptions.BadRequest;
using Store.G02.Domain.Exceptions.NotFound;
using Store.G02.Shared.ErrorModels;

namespace Store.G02.Web.Middleware
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
                if(context.Response.StatusCode == 404)
                {
                    context.Response.ContentType = "application/json";
                    var ResponseBody = new ErrorDetails()
                    {
                        StatusCode = context.Response.StatusCode,
                        ErrorMessage = "This Route Not Match Any EndPoint In Server Side !"
                    };
                    await context.Response.WriteAsJsonAsync(ResponseBody);
                }
            }
            catch(Exception exception)
            {

                context.Response.StatusCode = exception switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    BadRequestException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                context.Response.ContentType = "application/json";

                var ResponseBody = new ErrorDetails()
                {
                    StatusCode = context.Response.StatusCode,
                    ErrorMessage = exception.Message
                };

                await context.Response.WriteAsJsonAsync(ResponseBody);
            }
        }



    }
}
