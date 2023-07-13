using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movieapp.business.Abstract;
using movieapp.data.Concrete.EFCore;
using movieapp.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace userapp.webapi.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }


        // GET: api/<UserController>
        [HttpGet]
        public async Task<IActionResult> Get(string query)
        {
            var users = await userService.GetAll();
            if (query!=null)
            {
                users = users.Where(u => u.Username.ToLower().Contains(query.ToLower())).ToList();
            }
            return Ok(users);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await userService.GetById(id);
            if (user == null)
            {
                return NotFound(new { message = "There is no such a user like this" });
            }

            return Ok(user);
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserRegister u)
        {
            try
            {  
                await userService.Create(u);
                return Ok(u.User);
            }
            catch (ValidationException e)
            {
                List<string> errorMessages = e.Errors.Select(error => error.ErrorMessage).ToList();
                return BadRequest(new { message = e.Message});
            }

        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] User user)
        {
            try
            {
                User existingUser = await userService.GetById(id);

                if (existingUser == null)
                {
                    return NotFound(new { message = "There is no such a user like this" });
                }

                if (user.Password != null)
                {
                    user.Password = BC.HashPassword(user.Password);
                }
                
                Type modelType = user.GetType();
                Type userType = existingUser.GetType();

                PropertyInfo[] modelProperties = modelType.GetProperties();
                PropertyInfo[] userProperties = userType.GetProperties();

                foreach (var property in modelProperties)
                {
                    if (property.Name == "UserId")
                    {
                        continue;
                    }
                    var value = property.GetValue(user);
                    if (value != null)
                    {
                        var correspondingProperty = userProperties.FirstOrDefault(p => p.Name == property.Name);
                        if (correspondingProperty != null && correspondingProperty.CanWrite)
                        {
                            correspondingProperty.SetValue(existingUser, value);
                        }
                    }
                }
                await userService.Update(existingUser);
                return Ok(existingUser); 
            }
            catch (ValidationException e)
            {
                return BadRequest(new {message = e.Message });
            }
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await userService.GetById(id);
            if (user == null)
            {
                return NotFound(new { message = "There is no such a user like this" });
            }
            await userService.Delete(user);
            return Ok(new { message = "success "});
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User u)
        {
            if (await userService.Login(u))
            {
                return Ok(new { message = "Logged in."});
            }
            return BadRequest(new { message = "Invalid credentials." });
        }

        [HttpPost("{userId}/watched/{movieId}")]
        public async Task<IActionResult> Watch(int userId, int movieId)
        {
            try
            {
                await userService.AddUserWatched(userId, movieId);
                var movies = await userService.GetWatched(userId);
                var movie = movies.FirstOrDefault(m => m.MovieId == movieId);
                return Ok(movie);
            }
            catch (ValidationException e)
            {
                return BadRequest(new { message = e.Message });
            }
           
        }

        [HttpGet("{id}/watched")]
        public async Task<IActionResult> GetWatched(int id)
        {
            var watchedMovies = await userService.GetWatched(id);
            return Ok(watchedMovies);
        }
    }
}
