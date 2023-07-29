using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using HttpContextMoq;
using HttpContextMoq.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using movieapp.business.Abstract;
using movieapp.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using userapp.webapi.Controllers;
using Xunit;

namespace MovieApp.WebApi.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly IUserService userService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public UserControllerTests()
        {
            userService = A.Fake<IUserService>();
            httpContextAccessor = A.Fake<IHttpContextAccessor>();
        }

        [Fact]
        public async Task UserController_Get_ReturnOK()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
            string query = "testvalue";
            //Act
            var result = await controller.Get(query);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public void UserController_Profile_ReturnOK()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
            var user = A.Fake<User>(); // Simulate that the user is found
            var httpContext = A.Fake<HttpContext>();
            httpContext.Items = new Dictionary<object, object>
            {
                { "User", user }
            };
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);


            //Act
            var result = controller.Profile();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public void UserController_ProfileWithInvalidToken_ReturnNotFound()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
            var user = A.Fake<User>(); // Simulate that the user is found
            var httpContext = A.Fake<HttpContext>();
            httpContext.Items = new Dictionary<object, object>
            {
                { "User", null }
            };
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);


            //Act
            var result = controller.Profile();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async Task UserController_Post_ReturnOK()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
            var userRegister = A.Fake<UserRegister>();
            userRegister.User = A.Fake<User>();

            //Act
            var result = await controller.Post(userRegister);

            //Assert
            if (result is OkObjectResult okResult)
            {
                okResult.Value.Should().NotBeNull();
            }
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task UserController_PostWithException_ReturnBadRequest()
        {
            //Arrange
            var userRegister = A.Fake<UserRegister>();
            A.CallTo(() => userService.Create(userRegister)).Throws(new ValidationException("error"));
            var controller = new UserController(userService, httpContextAccessor);

            //Act
            var result = await controller.Post(userRegister);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async Task UserController_Put_ReturnOK()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
            var user = A.Fake<User>();
            var existingUser = A.Fake<User>();
            var httpContext = A.Fake<HttpContext>();
            httpContext.Items = new Dictionary<object, object>
            {
                { "User", existingUser }
            };
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);

            //Act
            var result = await controller.Put(user);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task UserController_PutWithInvalidToken_ReturnNotFound()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
            var user = A.Fake<User>();
            User existingUser = null;
            var httpContext = A.Fake<HttpContext>();
            httpContext.Items = new Dictionary<object, object>
            {
                { "User", existingUser }
            };
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);

            //Act
            var result = await controller.Put(user);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async Task UserController_PutWithException_ReturnBadRequest()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
            var user = A.Fake<User>();
            user.Password = "12345";
            var existingUser = A.Fake<User>();
            var httpContext = A.Fake<HttpContext>();
            httpContext.Items = new Dictionary<object, object>
            {
                { "User", existingUser }
            };
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);

            //Act
            var result = await controller.Put(user);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async Task UserController_DeleteWithValidId_ReturnOK()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
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
            var controller = new UserController(userService, httpContextAccessor);
            int id = 1;
            A.CallTo(() => userService.GetById(id)).Returns(null as User);
            //Act
            var result = await controller.Delete(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async Task UserController_Login_ReturnOK()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
            int id = 1;
            var user = A.Fake<User>();
            A.CallTo(() => userService.Login(user)).Returns(true);
           
            //Act
            var result = await controller.Login(user);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task UserController_LoginWithBadCredentials_ReturnBadRequest()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
            int id = 1;
            var user = A.Fake<User>();
            A.CallTo(() => userService.Login(user)).Returns(false);

            //Act
            var result = await controller.Login(user);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async Task UserController_Watch_ReturnOK()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
            int movieId = 1;
            var user = A.Fake<User>();
            var httpContext = A.Fake<HttpContext>();
            httpContext.Items = new Dictionary<object, object>
            {
                { "User", user }
            };
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);

            //Act
            var result = await controller.Watch(movieId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task UserController_WatchWithInvalidToken_ReturnNotFound()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
            var httpContext = A.Fake<HttpContext>();
            int movieId = 1;
            httpContext.Items = new Dictionary<object, object>
            {
                { "User", null }
            };
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);

            //Act
            var result = await controller.Watch(movieId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async Task UserController_WatchWithException_ReturnBadRequest()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
            var user = A.Fake<User>();
            int movieId = 1;
            var httpContext = A.Fake<HttpContext>();
            httpContext.Items = new Dictionary<object, object>
            {
                { "User", user }
            };
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);
            A.CallTo(() => userService.AddUserWatched(user.UserId, movieId)).Throws(new ValidationException("error message"));

            //Act
            var result = await controller.Watch(movieId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async Task UserController_GetWatched_ReturnOK()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
            var user = A.Fake<User>();
            var httpContext = A.Fake<HttpContext>();
            httpContext.Items = new Dictionary<object, object>
            {
                { "User", user }
            };
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);

            //Act
            var result = await controller.GetWatched();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task UserController_GetWatchedWithInvalidToken_ReturnNotFound()
        {
            //Arrange
            var controller = new UserController(userService, httpContextAccessor);
            var httpContext = A.Fake<HttpContext>();
            httpContext.Items["User"] = null;
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);

            //Act
            var result = await controller.GetWatched();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }


        [Fact]
        public async Task UserController_Logout_ReturnOK()
        {
            //Arrange
            var user = A.Fake<User>();
            var token = new Guid().ToString();
            var authorization = $"Bearer {token}";
            var httpContext = new HttpContextMock().SetupRequestHeaders(new Dictionary<string, StringValues>() 
            {
                {"Authorization", authorization }
            });
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);
            var controller = new UserController(userService, httpContextAccessor);

            //Act
            var result = await controller.Logout();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task UserController_LogoutWithInvalidToken_ReturnBadRequest()
        {
            //Arrange
            var user = A.Fake<User>();
            var token = new Guid().ToString();
            var authorization = $"Bearer {token}";
            var httpContext = new HttpContextMock();
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);
            var controller = new UserController(userService, httpContextAccessor);

            //Act
            var result = await controller.Logout();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }
    }
}
