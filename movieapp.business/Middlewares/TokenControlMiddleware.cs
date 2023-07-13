using Microsoft.AspNetCore.Http;
using movieapp.business.Abstract;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.Business.Middlewares
{
    // Middleware
    public class TokenControlMiddleware
    {
        private RequestDelegate _next;
        private IUserService userService; 

        public TokenControlMiddleware (RequestDelegate next, IUserService userService)
        {
            _next = next;
            this.userService = userService;
        }

        public async Task Invoke(HttpContext context)
        {
            // İşlem yapılacak controller ve metot adı
            /*  var controllerName = "users";
              var methodName = "SpecificMethod";

             
              if (context.Request.Path.StartsWithSegments($"/api/{controllerName}/{methodName}"))
              {
                 

                  await _next(context);
              }
              else
              {
             
                  await _next(context);
              } */

            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.ToString().Replace("Bearer ", "");

                // Token'ı doğrula
                if (Guid.TryParse(token, out Guid guid))
                {
                    // Kullanıcıyı eşleştir
                    var user = userService.GetFromCache(Guid.Parse(token));

                    // Özelleştirilmiş bir kimlik oluşturun ve kullanıcının kimliğini depolayın

                    var claimsIdentity = new ClaimsIdentity(new[] {new Claim("UserId", user.UserId.ToString()) });

                    context.User = new ClaimsPrincipal(claimsIdentity); 

                }
            }

            await _next(context);
        }
    }

    
}
