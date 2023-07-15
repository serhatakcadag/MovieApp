using Microsoft.AspNetCore.Http;
using movieapp.business.Abstract;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MovieApp.Business.Middlewares
{
    // Middleware
    public class TokenControlMiddleware : IMiddleware
    {
        private IUserService userService;

        public TokenControlMiddleware (IUserService userService)
        {
            this.userService = userService;
        }

        private string[] unprotectedGetRoutes = new string[] {"/api/movies"};

        private string[] unprotectedPostRoutes = new string[] { "/api/users", "/api/users/login" };

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Method == "POST")
            {
                foreach (var route in unprotectedPostRoutes)
                {
                    if (context.Request.Path.Value.ToLower() == route)
                    {
                        await next(context);
                    }
                }
            }

            if (context.Request.Method == "GET")
            {
                Console.WriteLine(context.Request.Path.Value);
                foreach (var route in unprotectedGetRoutes)
                {
                    if (context.Request.Path.StartsWithSegments(route))
                    {
                        await next(context);
                    }
                }
            }
           
            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.ToString().Replace("Bearer ", "");
                if (Guid.TryParse(token, out Guid result))
                {
                    var auth = userService.GetFromCache(Guid.Parse(token));
                    if (auth == null)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Unauthorized" }));
                        return;
                    }
                    context.Items["User"] = auth;
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Unauthorized" }));
                    return;
                }

            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Unauthorized" }));
                return;
            }
           
            await next(context);
        }
    }

    
}
