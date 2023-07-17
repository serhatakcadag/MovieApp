using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace MovieApp.Business.Middlewares
{
    static public class ConfigureExceptionHandlerExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder application)
        {
            application.UseExceptionHandler(builder =>
            {
                builder.Run(async context => {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = MediaTypeNames.Application.Json;

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    
                    var defaultMessage = contextFeature.Error.Message;

                    if (contextFeature.Error.InnerException != null)
                    {
                      
                        if (contextFeature.Error.InnerException.Data.Contains("Server Error Code"))
                        {   
                            var code = contextFeature.Error.InnerException.Data["Server Error Code"];
                            if ((int)code == 1062)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                if (contextFeature.Error.InnerException.Message.ToLower().Contains("email"))
                                {
                                    defaultMessage = "This email is already in use.";
                                }
                                else if (contextFeature.Error.InnerException.Message.ToLower().Contains("username"))
                                {
                                    defaultMessage = "This username is already in use.";
                                }
                                else
                                {
                                    defaultMessage = "Duplicated value error.";
                                }
                            }
                            else if ((int)code == 1452)
                            {
                                defaultMessage = "There is no record with this given ID";
                                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            }
                            else
                            {
                                defaultMessage = contextFeature.Error.InnerException.Message;
                            }

                        }
                        else
                        {
                            defaultMessage = contextFeature.Error.InnerException.Message;
                        }
                       
                    }

                    if (contextFeature != null)
                    {
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = defaultMessage,
                            Title = "Error"
                        })) ; 
                    }
                });
            });
        }
    }
}
