using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using Fixtures.API.Controllers;
using Fixtures.API.Data;
using Fixtures.API.DTOS;
using Fixtures.API.Models;
using Fixtures.API.Profile;
using FixturesTest.MockServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace FixturesTest.ControllersTest
{
    public class AuthControllerTest
    {
        private List<User> _users;
        private Mock<IAuthRepository> _authRepository;
        private IMapper _mapper;
        AutoMapperProfile profile;
        private AuthController _authController;
        private Mock<IConfiguration> _config;
        public AuthControllerTest()
        {
            var profile = new AutoMapperProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            _mapper = new Mapper(config);
            _config = new Mock<IConfiguration>();
            _config.SetupGet(s => s.GetSection("AppSettings:Token").Value).Returns("}}M7pjPxB^RSe/cyxHhM#bzdpV/q(y7!2vm3rq");

             _users = new List<User>(){
                new User() {
                    Id = 1,
                    Username = "olatunde",
                    UserRole = UserRole.Admin
                },
                new User() {
                    Id = 2,
                    Username = "kacha",
                    UserRole = UserRole.User
                },
                new User() {
                    Id = 3,
                    Username = "labule",
                    UserRole = UserRole.User
                },
                new User() {
                    Id = 4,
                    Username = "halima",
                    UserRole = UserRole.User
                }
            }; 
            
            _authRepository  = new Mock<IAuthRepository>();
            _authController = new AuthController(_authRepository.Object, _mapper, _config.Object);
        }
        [Fact]
        public async void Register_UserNameExists_ReturnBadRequest()
        {
            var testUserToRegister = new UserForRegistrationDto {
                Username = "labule",
                Password = "password",
                FavoriteTeam= "Chelsea FC"
            };

            _authRepository.Setup(rep => rep.UserExists(testUserToRegister.Username)).ReturnsAsync(true);

            var badRequestResult = await _authController.RegisterUser(testUserToRegister) as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal("Username exists", badRequestResult.Value);
        }
        [Fact]
        public async void Register_ValidUserDetails_ReturnCreatedAtRoute()
        {
            var testUserToRegister = new UserForRegistrationDto {
                Username = "emma",
                Password = "iko",
                FavoriteTeam= "Chelsea FC"
            };
            var userFromRepo = _mapper.Map<User>(testUserToRegister);
            _authRepository.Setup(rep => rep.UserExists(testUserToRegister.Username)).ReturnsAsync(false);
            _authRepository.Setup(rep => rep.RegisterUser(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(userFromRepo);

            var result = await _authController.RegisterUser(testUserToRegister) as CreatedAtRouteResult;
            Assert.NotNull(result);
            Assert.Equal("GetUser", result.RouteName);
            Assert.Equal("Users", result.RouteValues["controller"]);
            Assert.IsType<UserToReturnDto>(result.Value);

        }
        [Fact]
        public async void Register_ValidUserDetails_ConfirmUsernameOfReturnedUser()
        {
            var testUserToRegister = new UserForRegistrationDto {
                Username = "emma",
                Password = "iko",
                FavoriteTeam= "Chelsea FC"
            };
            var mappedTestUser = _mapper.Map<User>(testUserToRegister);
            var createdUser = _mapper.Map<User>(testUserToRegister);
            _authRepository.Setup(rep => rep.UserExists(testUserToRegister.Username)).ReturnsAsync(false);
            _authRepository.Setup(rep => rep.RegisterUser(It.IsAny<User>(), It.IsAny<string>())).Callback(() => this._users.Add(createdUser))
            .ReturnsAsync(createdUser);
           // _authRepository.Setup(rep => rep.GetUsers()).Callback(() => this._users.Add(createdUser)).ReturnsAsync(_users);

            var result = await _authController.RegisterUser(testUserToRegister) as CreatedAtRouteResult;

            var user = Assert.IsType<UserToReturnDto>(result.Value);
            Assert.Equal("emma", user.Username);
            Assert.Equal(5, this._users.Count);
        }
        [Fact]
        public async void Login_InvalidUserInput_ReturnBadRequestResult()
        {
            var testUser = new UserForLoginDto {
                Username = "",
                Password = null
            };
            var badRequestResult = await _authController.LoginUser(testUser) as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal("Username or Password is Invalid", badRequestResult.Value);
        }
        [Fact]
        public async void Login_UserNotInDb_ReturnUnauthorized()
        {
            var testUser = new UserForLoginDto {
                Username = "ifeoluwa",
                Password = "password"
            };
            _authRepository.Setup(m => m.LoginUser(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((User)null);
            var unAuthorized = await _authController.LoginUser(testUser);
            Assert.IsType<UnauthorizedResult>(unAuthorized);
        }

        [Fact]
        public async void Login_ValidUser_ReturnOkResponse()
        {
            var testUser = new UserForLoginDto {
                Username = "kacha",
                Password = "password"
            };
            var testUserFromRepo = _mapper.Map<User>(testUser);
            _authRepository.Setup(m => m.LoginUser(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(testUserFromRepo);
            var result = await _authController.LoginUser(testUser) as OkObjectResult;
            Assert.NotNull(result);
        }
    }
}