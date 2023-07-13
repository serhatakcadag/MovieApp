using Microsoft.EntityFrameworkCore;
using movieapp.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace movieapp.data.Concrete.EFCore
{
    public class MovieContext:DbContext
    {
        public MovieContext()
        {

        }
        public MovieContext(DbContextOptions<MovieContext> options) : base(options)
        {

        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserWatched> UserWatched  { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserWatched>().HasKey(u => new { u.UserId, u.MovieId });

            modelBuilder.Entity<UserWatched>()
                           .HasOne(uw => uw.User)
                           .WithMany(u => u.UserWatched)
                           .HasForeignKey(uw => uw.UserId);

            modelBuilder.Entity<UserWatched>()
                          .HasOne(uw => uw.Movie)
                          .WithMany(m => m.UserWatched)
                          .HasForeignKey(uw => uw.MovieId);

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        }
    }
}
