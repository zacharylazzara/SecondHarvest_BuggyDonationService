using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BuggyDonationService.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.Now;
            Console.WriteLine($"[{startTime}] Processing request: {context.Request.Method} {context.Request.Path}");
            
            await _next(context);
            
            var duration = DateTime.Now - startTime;
            Console.WriteLine($"Request completed in {duration.TotalMilliseconds}ms");
        }
    }
}