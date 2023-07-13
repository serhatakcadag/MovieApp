using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using movieapp.business.Abstract;
using movieapp.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MovieApp.WebApi.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MovieController : ControllerBase
    {

        private IMovieService movieService;

        public MovieController(IMovieService movieService)
        {
            this.movieService = movieService;
        }

        // GET: api/<MovieController>
        [HttpGet]
        public async Task<IActionResult> Get(string query)
        {
            var movies = await movieService.GetAll();
            if (query != null)
            {
                movies = movies.Where(m => m.Title.ToLower().Contains(query.ToLower())).ToList();
            }
            return Ok(movies);
        }

       

        // GET api/<MovieController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var movie = await movieService.GetById(id);
            if (movie == null)
            {
               return NotFound(new {message = "There is no such a movie like this" });
            }

            return Ok(movie);
        }

        // POST api/<MovieController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Movie movie)
        {
            try
            {
                await movieService.Create(movie);
                return Ok(movie);
            }
            catch (ValidationException e)
            {
                List<string> errorMessages = e.Errors.Select(error => error.ErrorMessage).ToList();
                return BadRequest(new { messages = errorMessages});
            }
           
        }

        // PUT api/<MovieController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Movie movie)
        {
            try
            {
                Movie existingMovie = await movieService.GetById(id);

                if (existingMovie == null)
                {
                    return NotFound(new { message = "There is no such a movie like this" });
                }

                Type modelType = movie.GetType();
                Type movieType = existingMovie.GetType();

                PropertyInfo[] modelProperties = modelType.GetProperties();
                PropertyInfo[] userProperties = movieType.GetProperties();

                foreach (var property in modelProperties)
                {
                    if (property.Name == "MovieId")
                    {
                        continue;
                    }
                    var value = property.GetValue(movie);
                    if (value != null)
                    {
                        var correspondingProperty = userProperties.FirstOrDefault(p => p.Name == property.Name);
                        if (correspondingProperty != null && correspondingProperty.CanWrite)
                        {
                            correspondingProperty.SetValue(existingMovie, value);
                        }
                    }
                }
                await movieService.Update(existingMovie);
                return Ok(existingMovie);
            }
            catch (ValidationException e)
            {
                return BadRequest(new {message = e.Message });
            }
        }

        // DELETE api/<MovieController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await movieService.GetById(id);
            if (movie == null)
            {
                return NotFound(new { message = "There is no such a movie like this" });
            }
            await movieService.Delete(movie);
            return Ok(new { message = "success " });
        }
    }
}
