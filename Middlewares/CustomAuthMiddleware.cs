using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pet_s_Land.Middlewares
{
    public class CustomAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);  // Process request first

            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized) // 401
            {
                context.Response.ContentType = "application/json";
                var response = new { error = "Unauthorized", message = "Authentication required." };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden) // 403 
            {
                context.Response.ContentType = "application/json";
                var response = new { error = "Forbidden", message = "You do not have permission to access this resource." };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
