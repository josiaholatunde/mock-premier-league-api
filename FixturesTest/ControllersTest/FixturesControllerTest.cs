using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Fixtures.API.Controllers;
using Fixtures.API.Data;
using Fixtures.API.DTOS;
using Fixtures.API.Helpers;
using Fixtures.API.Models;
using Fixtures.API.Profile;
using FixturesTest.MockServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace FixturesTest.ControllersTest
{
    public class FixturesControllerTest
    {
        private IMapper _mapper;
        AutoMapperProfile profile;
        private FixturesController _fixturesController;
        private Mock<IPremierLeagueRepository> _repository;
        private Mock<IConfiguration> _config;
        public FixturesControllerTest()
        {
            var profile = new AutoMapperProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            _mapper = new Mapper(config);
            _repository = new Mock<IPremierLeagueRepository>();
            _fixturesController = new FixturesController(_repository.Object, _mapper);
        }

        [Fact]
        public async void GetFixtures_WhenCalledWithNullTeamIdAndPendingFixturesParams_ReturnPendingFixtures()
        {
            var mockObjects = new List<Fixture>(){
                new  Fixture {
                    HomeTeamId = 1,
                    HomeTeamScore = 0,
                    AwayTeamId = 0,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(2019,2,11,17,30, 0),
                },
                new  Fixture {
                    HomeTeamId = 2,
                    HomeTeamScore = 0,
                    AwayTeamId = 1,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(2019,1,11,17,30, 0),
                },
                new  Fixture {
                    HomeTeamId = 2,
                    HomeTeamScore = 0,
                    AwayTeamId = 1,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(1972,4,11,17,30, 0),
                },
            };
            var userParams = new UserParams {
                teamId = null,
                IsFixtureCompleted = false
            };
            _repository.Setup(rep => rep.GetFixtures(userParams)).ReturnsAsync(
                mockObjects.Where(fx => DateTime.Now <= fx.KickOffTime.AddMinutes(90)).ToList());

            var result = await _fixturesController.GetFixtures(userParams) as OkObjectResult;

            _repository.Verify(rep => rep.GetFixtures(userParams), Times.Once());
            var teams = Assert.IsType<List<Fixture>>(result.Value);
            Assert.True(teams.Count == 0);
        }
        [Fact]
        public async void GetFixtures_WhenCalledWithNullTeamIdAndIsFixtureCompleted_ReturnCompletedFixtures()
        {
            var mockObjects = new List<Fixture>(){
                new  Fixture {
                    HomeTeamId = 2,
                    HomeTeamScore = 0,
                    AwayTeamId = 1,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(2019,4,11,17,30, 0),
                    IsFixtureCompleted = false
                },
                new  Fixture {
                   HomeTeamId = 2,
                    HomeTeamScore = 0,
                    AwayTeamId = 1,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(2019,4,11,17,30, 0),
                    IsFixtureCompleted = false
                },
                new  Fixture {
                    HomeTeamId = 2,
                    HomeTeamScore = 0,
                    AwayTeamId = 1,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(1972,4,11,17,30, 0),
                    IsFixtureCompleted = true
                },
            };
            var userParams = new UserParams {
                teamId = null,
                IsFixtureCompleted = false
            };
            _repository.Setup(rep => rep.GetFixtures(userParams)).ReturnsAsync(
                mockObjects.Where(fx => DateTime.Now > fx.KickOffTime.AddMinutes(90) && fx.IsFixtureCompleted == true).ToList());

            var result = await _fixturesController.GetFixtures(userParams) as OkObjectResult;

            _repository.Verify(rep => rep.GetFixtures(userParams), Times.Once());
            var teams = Assert.IsType<List<Fixture>>(result.Value);
            Assert.True(teams.Count == 1);
        }
        [Fact]
        public async void GetFixtures_WhenCalledWithValidTeamIdAndIsFixtureCompleted_ReturnCompletedFixtures()
        {
            var mockObjects = new List<Fixture>(){
                new  Fixture {
                   HomeTeamId = 2,
                    HomeTeamScore = 0,
                    AwayTeamId = 1,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(2019,4,11,17,30, 0),
                },
                new  Fixture {
                    HomeTeamId = 2,
                    HomeTeamScore = 0,
                    AwayTeamId = 5,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(2019,4,11,17,30, 0),
                },
                new  Fixture {
                    HomeTeamId = 2,
                    HomeTeamScore = 0,
                    AwayTeamId = 1,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(1972,4,11,17,30, 0),
                },
            };
            var userParams = new UserParams {
                teamId = 1,
                IsFixtureCompleted = true
            };
            _repository.Setup(rep => rep.GetFixtures(userParams)).ReturnsAsync(
                mockObjects.Where(fx => DateTime.Now > fx.KickOffTime.AddMinutes(90)).ToList());

            var result = await _fixturesController.GetFixtures(userParams) as OkObjectResult;

            _repository.Verify(rep => rep.GetFixtures(userParams), Times.Once());
            var teams = Assert.IsType<List<Fixture>>(result.Value);
            Assert.True(teams.Count == 1);
        }
        [Fact]
        public async void GetFixtures_WhenCalledWithValidTeamIdAndIsFixturePending_ReturnPending()
        {
            var mockObjects = new List<Fixture>(){
                new  Fixture {
                   HomeTeamId = 2,
                    HomeTeamScore = 0,
                    AwayTeamId = 1,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(2019,4,11,17,30, 0),
                },
                new  Fixture {
                    HomeTeamId = 3,
                    HomeTeamScore = 0,
                    AwayTeamId = 1,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(2019,4,11,17,30, 0),
                },
                new  Fixture {
                    HomeTeamId = 1,
                    HomeTeamScore = 0,
                    AwayTeamId = 2,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(1972,4,11,17,30, 0),
                },
            };
            var userParams = new UserParams {
                teamId = 1,
                IsFixtureCompleted = false
            };
            _repository.Setup(rep => rep.GetFixtures(userParams)).ReturnsAsync(
                mockObjects.Where(fx => DateTime.Now <= fx.KickOffTime.AddMinutes(90)).ToList());

            var result = await _fixturesController.GetFixtures(userParams) as OkObjectResult;

            _repository.Verify(rep => rep.GetFixtures(userParams), Times.Once());
            var teams = Assert.IsType<List<Fixture>>(result.Value);
            Assert.True(teams.Count == 2);
        }

         [Fact]
        public async void CreateFixtures_InvalidTeamId_ReturnBadRequest()
        {
            var fixtureToCreateDto = new  FixtureToCreate {
                   HomeTeamId = 99,
                   HomeTeamScore = 0,
                   AwayTeamId = 98,
                   AwayTeamScore = 0,
                   KickOffTime =  new System.DateTime(2019,4,11,17,30, 0),
                };
            _repository.Setup(rep => rep.GetTeam(fixtureToCreateDto.HomeTeamId)).ReturnsAsync((Team)null);
            _repository.Setup(rep => rep.GetTeam(fixtureToCreateDto.AwayTeamId)).ReturnsAsync((Team)null);

            var result = await _fixturesController.CreateTeamFixture(fixtureToCreateDto) as BadRequestObjectResult;
            Assert.NotNull(result);
            Assert.Equal("Home Team specified does not exist", result.Value);
            _repository.Verify();
        }
         [Fact]
        public async void CreateFixtures_ValidTeamId_ReturnCreatedAtRoute()
        {
            var fixtureToCreateDto = new  FixtureToCreate {
                   HomeTeamId = 2,
                    HomeTeamScore = 0,
                    AwayTeamId = 1,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(2019,2,11,17,30, 0),
                };
                var homeTeamFromRepo = new Team {
                    Name = "Arsenal FC"
                };
                var awayTeamFromRepo = new Team {
                    Name = "Chelsea FC"
                };
            _repository.Setup(rep => rep.GetTeam(fixtureToCreateDto.HomeTeamId)).ReturnsAsync(homeTeamFromRepo);
            _repository.Setup(rep => rep.GetTeam(fixtureToCreateDto.AwayTeamId)).ReturnsAsync(awayTeamFromRepo);
            _repository.Setup(rep => rep.SaveAllChangesAsync()).ReturnsAsync(true);

            var result = await _fixturesController.CreateTeamFixture(fixtureToCreateDto) as CreatedAtRouteResult;
            Assert.NotNull(result);
            Assert.Equal("GetFixture", result.RouteName);
            Assert.Equal(fixtureToCreateDto.Id, result.RouteValues["id"]);
            Assert.IsType<FixtureToReturn>(result.Value);
        }
         [Fact]
        public async void EditFixture_TeamIdOnRouteDoesNotMatchTeamIdOnRequestBody_ReturnBadRequest()
        {
            var fixtureToEditDto = new  FixtureToCreate {
                    Id = 1,
                   HomeTeamId = 99,
                    HomeTeamScore = 0,
                    AwayTeamId = 1,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(2020,4,11,17,30, 0),
                };
            var fixtureId = 77;
            var fixtureFromRepo= _mapper.Map<Fixture>(fixtureToEditDto);
            _repository.Setup(rep => rep.GetFixture(fixtureId)).ReturnsAsync(fixtureFromRepo);

            var result = await _fixturesController.EditFixture(fixtureId, fixtureToEditDto) as BadRequestObjectResult;
            Assert.NotNull(result);
            Assert.Equal($"Fixture Id on the route {fixtureId} does not match fixtureId on the body of request {fixtureToEditDto.Id}", result.Value);
            _repository.Verify();
        }
         [Fact]
        public async void EditFixture_ValidTeamId_ReturnNoContent()
        {
            var fixtureToEditDto = new  FixtureToCreate {
                    Id = 1,
                    HomeTeamId = 2,
                    HomeTeamScore = 0,
                    AwayTeamId = 3,
                    AwayTeamScore = 0,
                    KickOffTime =  new System.DateTime(2022,4,11,17,30, 0)
                };
                var homeTeamFromRepo = new Team {
                    Name = "Arsenal FC",
                    Id = 1
                };
                var awayTeamFromRepo = new Team {
                    Name = "Chelsea FC",
                    Id = 3
                };
              var fixtureFromRepo = _mapper.Map<Fixture>(fixtureToEditDto);
            _repository.Setup(rep => rep.GetTeam(fixtureToEditDto.HomeTeamId)).ReturnsAsync(homeTeamFromRepo);
            _repository.Setup(rep => rep.GetTeam(fixtureToEditDto.AwayTeamId)).ReturnsAsync(awayTeamFromRepo);
            _repository.Setup(rep => rep.GetFixture(fixtureToEditDto.Id)).ReturnsAsync(fixtureFromRepo);
            _repository.Setup(rep => rep.SaveAllChangesAsync()).ReturnsAsync(true);

            var result = await _fixturesController.EditFixture(fixtureToEditDto.Id, fixtureToEditDto);

            _repository.Verify(rep=> rep.SaveAllChangesAsync(), Times.Once());
            Assert.IsType<NoContentResult>(result);
        }
    }
}