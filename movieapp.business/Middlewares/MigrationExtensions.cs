using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using movieapp.data.Concrete.EFCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieApp.Business.Middlewares
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<MovieContext>();

            dbContext.Database.Migrate();
        }
    }
}
