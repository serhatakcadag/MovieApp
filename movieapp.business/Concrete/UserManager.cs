using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Caching.Memory;
using movieapp.business.Abstract;
using movieapp.data.Abstract;
using movieapp.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace movieapp.business.Concrete
{
    public class UserManager : IUserService
    {
       
        private IUserRepository userRepository;
        private IValidator<UserRegister> userValidator;
        private readonly IMemoryCache cache;

        public UserManager(IUserRepository userRepository, IValidator<UserRegister> userValidator, IMemoryCache cache)
        {
            this.userValidator = userValidator;
            this.userRepository = userRepository;
            this.cache = cache;
        }

        public async Task AddUserWatched(int userId, int movieId)
        {
            var movies = await userRepository.GetWatched(userId);
            var movie = movies.FirstOrDefault(m => m.MovieId == movieId);
            if (movie != null)
            {
                throw new ValidationException("The movie is already watched.");
            }
            await userRepository.AddUserWatched(userId, movieId);
        }

        public async Task Create(UserRegister entity)
        {
            ValidationResult validationResult = userValidator.Validate(entity);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
      
            entity.User.Password = BC.HashPassword(entity.User.Password);

            await userRepository.Create(entity.User);
        }

        public async Task Delete(User entity)
        {
            await userRepository.Delete(entity);
        }

        public async Task<List<User>> GetAll()
        {
            return await userRepository.GetAll();
        }

        public async Task<User> GetById(int id)
        {
            return await userRepository.GetById(id);
        }

        public async Task<List<Movie>> GetWatched(int id)
        {
            return await userRepository.GetWatched(id);
        }

        public async Task<bool> Login(User entity)
        {
            var users = await userRepository.GetAll();
            var user = users.FirstOrDefault(u=> u.Email == entity.Email);
            if (user == null || !BC.Verify(entity.Password, user.Password))
            {
                return false;
            }
            return true;
        }

        public async Task Update(User entity)
        {
            UserRegister userRegister = new UserRegister() { User = entity, PasswordConfirmation = entity.Password };
            ValidationResult validationResult = userValidator.Validate(userRegister);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            } 
        
            await userRepository.Update(entity);
        }

        public async Task StoreInCache(Guid token, User user)
        {
              var users = await userRepository.GetAll();
              var fullUser = users.FirstOrDefault(u => u.Email == user.Email);
              cache.Set(token, fullUser, TimeSpan.FromMinutes(20));
        }

        public User GetFromCache(Guid token)
        {
            var user = cache.Get<User>(token);
            return user;
        }
    }
}
