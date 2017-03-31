
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CoreSecuirtyToekn.Middleware
{
    public class AuthMiddleWare
    {
        private readonly RequestDelegate _next;

        public  AuthMiddleWare(RequestDelegate next)
        {
            _next = next;;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var url = httpContext.Connection;
            var l = httpContext.Request.Path;
            if(httpContext.Request.Path.HasValue && httpContext.Request.Path == "/Token")
            {
                await httpContext.Response.WriteAsync("{Name:'Something', Text:'somethingelse'}");

            } else {

            Console.WriteLine($"Request for {httpContext.Request.Path} received ({httpContext.Request.ContentLength ?? 0} bytes)");
            
            // Call the next middleware delegate in the pipeline 
            await _next.Invoke(httpContext);
            }
        }
    }    
}