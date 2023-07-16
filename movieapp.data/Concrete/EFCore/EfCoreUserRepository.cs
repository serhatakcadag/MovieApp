using Microsoft.EntityFrameworkCore;
using movieapp.data.Abstract;
using movieapp.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace movieapp.data.Concrete.EFCore
{
    public class EfCoreUserRepository : EfCoreGenericRepository<User, MovieContext>, IUserRepository
    {
        private readonly MovieContext context;
        public EfCoreUserRepository(MovieContext context) : base(context)
        {
            this.context = context;
        }
        public async Task AddUserWatched(int userId, int movieId)
        {
           
                var userWatched = new UserWatched() { UserId = userId, MovieId = movieId };
                await context.UserWatched.AddAsync(userWatched);
                await context.SaveChangesAsync();
            
        }

        public async Task<List<Movie>> GetWatched(int id)
        {
         
                var watchedMovies = await context.UserWatched
                                    .Include(uw => uw.Movie)
                                    .Where(uw => uw.UserId == id)
                                    .Select(uw => uw.Movie)
                                    .ToListAsync();

                return watchedMovies;
            
        }
    }
}
