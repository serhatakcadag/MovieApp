using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using movieapp.business.Abstract;
using movieapp.entity;
using MovieApp.WebApi.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MovieApp.WebApi.Tests.Controllers
{
    public class MovieControllerTests
    {

        private readonly IMovieService movieService;
        public MovieControllerTests()
        {
            movieService = A.Fake<IMovieService>();
        }

        [Fact]
        public async Task MovieController_Get_ReturnOK()
        {
            //Arrange
            var controller = new MovieController(movieService);
            string query = "testvalue";
            //Act
            var result = await controller.Get(query);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task MovieController_GetWithValidId_ReturnOK()
        {
            //Arrange
            var controller = new MovieController(movieService);
            var movie = A.Fake<Movie>();
            int id = 1;
            
            //Act
            var result = await controller.Get(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task MovieController_GetWithInvalidId_ReturnNotFound()
        {
            //Arrange
            var controller = new MovieController(movieService);
            var movie = A.Fake<Movie>();
            int id = 1;
            A.CallTo(() => movieService.GetById(id)).Returns(null as Movie);
            //Act
            var result = await controller.Get(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async Task MovieController_Post_ReturnOK()
        {
            //Arrange
            var controller = new MovieController(movieService);
            var movie = A.Fake<Movie>();

            //Act
            var result = await controller.Post(movie);

            //Assert
            if (result is OkObjectResult okResult)
            {
                okResult.Value.Should().NotBeNull();
            }
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task MovieController_PostWithException_ReturnBadRequest()
        {
            //Arrange
            var controller = new MovieController(movieService);
            var movie = A.Fake<Movie>();
            A.CallTo(() => movieService.Create(movie)).Throws(new ValidationException("error"));

            //Act
            var result = await controller.Post(movie);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async Task MovieController_PutWithValidId_ReturnOK()
        {
            //Arrange
            var controller = new MovieController(movieService);
            var movie = A.Fake<Movie>();
            int id = 1;
            //Act
            var result = await controller.Put(id, movie);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task MovieController_PutWithInvalidId_ReturnNotFound()
        {
            //Arrange
            var controller = new MovieController(movieService);
            var movie = A.Fake<Movie>();
            int id = 1;
            A.CallTo(() => movieService.GetById(id)).Returns(null as Movie);
            //Act
            var result = await controller.Put(id, movie);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async Task MovieController_PutWithException_ReturnBadRequest()
        {
            //Arrange
            var controller = new MovieController(movieService);
            var movie = A.Fake<Movie>();
            int id = 1;
            A.CallTo(() => movieService.Update(movie)).Throws(new ValidationException("Validation error"));
            //Act
            var result = await controller.Put(id, movie);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async Task MovieController_DeleteWithValidId_ReturnOK()
        {
            //Arrange
            var controller = new MovieController(movieService);
            int id = 1;
            //Act
            var result = await controller.Delete(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task MovieController_DeleteWithInvalidId_ReturnNotFound()
        {
            //Arrange
            var controller = new MovieController(movieService);
            int id = 1;
            A.CallTo(() => movieService.GetById(id)).Returns(null as Movie);
            //Act
            var result = await controller.Delete(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }

    }
}
