using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movieapp.business.Abstract;
using movieapp.data.Concrete.EFCore;
using movieapp.entity;
using MovieApp.Business.Middlewares;
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
        private readonly IUserService userService;
        private readonly IHttpContextAccessor httpContextAccessor;  
        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            this.userService = userService;
            this.httpContextAccessor = httpContextAccessor;
        }


        // GET: api/<UserController>
        [HttpGet]
        public async Task<IActionResult> Get(string query, int page = 1, int limit = 2)
        {
            var users = await userService.GetAll();

            if (query != null)
            {
                users = users.Where(u => u.Username.ToLower().Contains(query.ToLower())).ToList();
            }

            int total = users.Count;
            int skip = (page - 1) * limit;

            var pagedItems = users.Skip(skip).Take(limit).ToList();
                            
            int? next = null;
            int? prev = null;
            if (page*limit < total)
            {
                next = page + 1;
            }
            if (page*limit > limit)
            {
                prev = page - 1; 
            }

            var pagination = new { next = next, prev = prev };

           
            return Ok(new {pagination = pagination, users = pagedItems });
        }

        // GET api/<UserController>/5
        [HttpGet("profile")]
        public IActionResult Profile()
        {

            if (httpContextAccessor.HttpContext.Items.TryGetValue("User", out var user))
            {
                if (user == null)
                {
                    return NotFound(new { message = "There is no such a user like this" });
                }
                
                return Ok(user);
            }
            else
            {
                return NotFound(new { message = "There is no such a user like this" });
            }
           
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

        // PUT api/<UserController>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] User user)
        {
            try
            {
                if (httpContextAccessor.HttpContext.Items.TryGetValue("User", out var existingUser))
                {
                    if (existingUser == null)
                    {
                        return NotFound(new { message = "There is no such a user like this" });
                    }
                    if (user.Password != null)
                    {
                        if (user.Password.Length < 6)
                        {
                            throw new ValidationException("Password field cannot be less than 6 characters");
                        }
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
                    await userService.Update((User)existingUser);
                    return Ok(existingUser);
                }
                else
                {
                    return NotFound(new { message = "There is no such a user like this" });
                }
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
                var guid = Guid.NewGuid();
                await userService.StoreInCache(guid, u);
                return Ok(new { message = "Logged in.", token = guid});
            }
            return BadRequest(new { message = "Invalid credentials." });
        }

        [HttpPost("watched/{movieId}")]
        public async Task<IActionResult> Watch(int movieId)
        {
            try
            {
                if (httpContextAccessor.HttpContext.Items.TryGetValue("User", out var user))
                {
                    if (user == null)
                    {
                        return NotFound(new { message = "There is no such a user like this" });
                    }
                    User existingUser = (User)user;
                    await userService.AddUserWatched(existingUser.UserId, movieId);
                    var movies = await userService.GetWatched(existingUser.UserId);
                    var movie = movies.FirstOrDefault(m => m.MovieId == movieId);
                    return Ok(movie);
                }
                else
                {
                    return NotFound(new { message = "There is no such a user like this" });
                }
               
            }
            catch (ValidationException e)
            {
                return BadRequest(new { message = e.Message });
            }
           
        }

        [HttpGet("watched")]
        public async Task<IActionResult> GetWatched()
        {
            if (httpContextAccessor.HttpContext.Items.TryGetValue("User", out var user))
            {
                if (user == null)
                {
                    return NotFound(new { message = "There is no such a user like this" });
                }
                User existingUser = (User)user;
                var watchedMovies = await userService.GetWatched(existingUser.UserId);
                return Ok(watchedMovies);
            }
            else
            {
                return NotFound(new { message = "There is no such a user like this" });
            }
           
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.ToString().Replace("Bearer ", "");
                userService.Logout(Guid.Parse(token));
                return Ok(new { message = "Logged out" });
            }
            return BadRequest(new { message = "Invalid credentials." });
        }
    }
}
