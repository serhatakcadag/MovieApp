using movieapp.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace movieapp.data.Abstract
{
    public interface IUserRepository : IRepository<User>
    {
        Task AddUserWatched(int userId, int movieId);
        Task<List<Movie>> GetWatched(int id);
    }
}
