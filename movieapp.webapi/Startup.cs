using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using movieapp.business.Abstract;
using movieapp.business.Concrete;
using movieapp.business.Validator;
using movieapp.data.Abstract;
using movieapp.data.Concrete.EFCore;
using movieapp.entity;
using MovieApp.Business.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace movieapp.webapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MovieContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("MySqlConnectionString"));
            });
            services.AddScoped<IMovieRepository, EfCoreMovieRepository>();
            services.AddScoped<IMovieService, MovieManager>();
            services.AddScoped<IUserRepository, EfCoreUserRepository>();
            services.AddScoped<IUserService, UserManager>();

            services.AddControllers().AddFluentValidation(x => { x.AutomaticValidationEnabled = false; }).AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddScoped<IValidator<Movie>, MovieValidator>();
            services.AddScoped<IValidator<UserRegister>, UserValidator>();

            services.AddTransient<TokenControlMiddleware>();
            services.AddMemoryCache();
         
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
           
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // app.UseTokenControlMiddleware();
            app.ConfigureExceptionHandler();

            app.UseMiddleware<TokenControlMiddleware>();


           /* app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/users"), appBuilder =>
            {
                appBuilder.UseMiddleware<TokenControlMiddleware>();
            });*/

           

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
