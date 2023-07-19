using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
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
                        Console.WriteLine(contextFeature.Error.InnerException.GetType());
                        if(contextFeature.Error.InnerException is MySqlException mySqlException)
                        {
                            // özel exception paketine bak
                            Console.WriteLine(mySqlException.Number);
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                           
                            if (mySqlException.Number == 1062)
                            {
                                if (mySqlException.Message.ToLower().Contains("email"))
                                {
                                    defaultMessage = "This email is already in use.";
                                }
                                else if (mySqlException.Message.ToLower().Contains("username"))
                                {
                                    defaultMessage = "This username is already in use.";
                                }
                                else
                                {
                                    defaultMessage = "Duplicated value error.";
                                }
                            }
                            else if(mySqlException.Number == 1452)
                            {
                                defaultMessage = "There is no record with this given id.";
                            }
                            else
                            {
                                defaultMessage = mySqlException.Message;
                            }
                        }
                        else if (contextFeature.Error.InnerException is SqlException sqlException)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                            if (sqlException.Number == 2601)
                            {
                                if (sqlException.Message.ToLower().Contains("email"))
                                {
                                    defaultMessage = "This email is already in use.";
                                }
                                else if (sqlException.Message.ToLower().Contains("username"))
                                {
                                    defaultMessage = "This username is already in use.";
                                }
                                else
                                {
                                    defaultMessage = "Duplicated value error.";
                                }
                            }
                            else if (sqlException.Number == 547)
                            {
                                defaultMessage = "There is no record with this given id.";
                            }
                            else
                            {
                                defaultMessage = sqlException.Message;
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
