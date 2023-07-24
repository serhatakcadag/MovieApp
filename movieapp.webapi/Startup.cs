using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using movieapp.business.Abstract;
using movieapp.business.Concrete;
using movieapp.business.Validator;
using movieapp.data.Abstract;
using movieapp.data.Concrete.EFCore;
using movieapp.entity;
using MovieApp.Business.Middlewares;
using System;

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
            var provider = Configuration["Provider"];

            switch (provider)
            {
                case "SqlServer":
                    services.AddDbContext<MovieContext>(options =>
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnectionString"),
                               x => x.MigrationsAssembly("MovieApp.SqlServerMigrations"));
                    });
                    break;
                case "MySql":
                    services.AddDbContext<MovieContext>(options =>
                    {
                        options.UseMySql(Configuration.GetConnectionString("MySqlConnectionString"),
                            x => x.MigrationsAssembly("MovieApp.MySqlMigrations"));
                    });
                    break;
                default:
                    throw new InvalidOperationException("Invalid database type specified in appsettings.json.");
            }


            /*services.AddDbContext<MovieContext>(options =>
             {
                 options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnectionString"),
                        x => x.MigrationsAssembly("MovieApp.SqlServerMigrations"));
             }); */
           /* services.AddDbContext<MovieContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("MySqlConnectionString"), 
                    x => x.MigrationsAssembly("MovieApp.MySqlMigrations"));
            });*/

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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MovieApp API", 
                    Version = "v1",
                    Description = "A basic movie app API with ASP.NET Core" 
                });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Guid Authorization header using the Bearer scheme.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                };

                c.AddSecurityRequirement(securityRequirement); 


            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.ApplyMigrations();
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MovieApp");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // app.UseTokenControlMiddleware();
            app.ConfigureExceptionHandler();

           // app.UseMiddleware<TokenControlMiddleware>();


            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/users"), appBuilder =>
            {
                appBuilder.UseMiddleware<TokenControlMiddleware>();
            });
            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/movies"), appBuilder =>
            {
                appBuilder.UseMiddleware<TokenControlMiddleware>();
            });



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
