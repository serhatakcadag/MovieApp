using movieapp.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace movieapp.business.Abstract
{
    public interface IUserService
    {
        Task<List<User>> GetAll();
        Task<User> GetById(int id);
        Task Create(UserRegister entity);
        Task Update(User entity);
        Task Delete(User entity);
        Task<bool> Login(User entity);
        Task AddUserWatched(int userId, int movieId);
        Task<List<Movie>> GetWatched(int id);
        Task StoreInCache(Guid token, User user);
        User GetFromCache(Guid token);
        void Logout(Guid guid);
    }
}
