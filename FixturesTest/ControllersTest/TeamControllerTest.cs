using System.Collections.Generic;
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
    public class TeamControllerTest
    {
        private IMapper _mapper;
        AutoMapperProfile profile;
        private TeamsController _teamsController;
        private Mock<IPremierLeagueRepository> _repository;
        private Mock<IConfiguration> _config;
        public TeamControllerTest()
        {
            var profile = new AutoMapperProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            _mapper = new Mapper(config);
            _repository = new Mock<IPremierLeagueRepository>();
            _teamsController = new TeamsController(_repository.Object, _mapper);
        }

        [Fact]
        public async void GetTeams_WhenCalled_ExpectRepoGetTeamsToBeCalledOnce()
        {
            var mockObjects = new List<Team>(){
                new  Team {
                    Name = "Chelsea FC",
                    LocationCity = "London West",
                    LocationCountry = "United Kingdom"
                },
                new  Team {
                    Name = "Arsenal FC",
                    LocationCity = "London North",
                    LocationCountry = "United Kingdom"
                },
                new  Team {
                    Name = "Tottenham FC",
                    LocationCity = "London West",
                    LocationCountry = "United Kingdom"
                }
            };
            _repository.Setup(rep => rep.GetTeams()).ReturnsAsync(mockObjects);

            var result = await _teamsController.GetTeams() as OkObjectResult;

            _repository.Verify(rep => rep.GetTeams(), Times.Once());
            var teams = Assert.IsType<List<Team>>(result.Value);
            Assert.True(teams.Count == 3);
        }

        [Fact]
        public async void CreateTeam_CalledWithATeamThatExists_ReturnsBadRequest()
        {
            var teamExists = new  TeamToCreateDto {
                    Name = "Chelsea FC",
                    FanStrength = 5000
            };
            _repository.Setup(rep => rep.TeamExists(teamExists.Name)).ReturnsAsync(true);

            var result =  await _teamsController.CreateTeam(teamExists) as BadRequestObjectResult;
            Assert.NotNull(result);
            Assert.Equal("Team name exists", result.Value);
            
        }
        [Fact]
        public async void CreateTeam_CalledWithAValidTeam_ReturnsCreatedAtRoute()
        {
            var validTeam = new  TeamToCreateDto {
                    Id = 6,
                    Name = "Burnley FC",
                    FanStrength = 5000
            };
             _repository.Setup(rep => rep.TeamExists(validTeam.Name)).ReturnsAsync(false);
            _repository.Setup(rep => rep.SaveAllChangesAsync()).ReturnsAsync(true);

            var result =  await _teamsController.CreateTeam(validTeam) as CreatedAtRouteResult;

            Assert.NotNull(result);
            Assert.Equal("GetTeam", result.RouteName);
            Assert.Equal(validTeam.Name, result.RouteValues["name"]);
            Assert.Equal(validTeam.Id, result.RouteValues["id"]);
            Assert.IsType<TeamToReturnDto>(result.Value);
        }
        [Fact]
        public async void CreateTeam_CalledWithAValidTeam_FailedToSaveResultDueToServerError()
        {
            var validTeam = new  TeamToCreateDto {
                    Name = "Burnley FC",
                    FanStrength = 5000
            };
             _repository.Setup(rep => rep.TeamExists(validTeam.Name)).ReturnsAsync(false);
            _repository.Setup(rep => rep.SaveAllChangesAsync()).ReturnsAsync(false);

            var result =  await _teamsController.CreateTeam(validTeam) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("An Error occurred while creating Team", result.Value);
        }

         [Fact]
        public async void EditTeam_CalledWithAnInvalidTeamId_ReturnsBadRequest()
        {
            var inValidTeam = new  TeamToCreateDto {
                Id = 55,
                Name = "Burnley FC",
                FanStrength = 5000
            };
            var invalidTeamId = 79;
            var result =  await _teamsController.EditTeam(invalidTeamId, inValidTeam) as BadRequestObjectResult;
            Assert.NotNull(result);
            Assert.Equal("Team id does not match id on body of request", result.Value);
        }
        [Fact]
        public async void EditTeam_CalledWithAnIdThatDoesNotExist_ReturnsBadRequest()
        {
            var inValidTeamEdit = new  TeamToCreateDto {
                Id = 55,
                Name = "Burnley FC",
                FanStrength = 5000
            };
            var invalidTeamId = 55;
            _repository.Setup(rep => rep.GetTeam(invalidTeamId)).ReturnsAsync((Team)null);

            var result =  await _teamsController.EditTeam(invalidTeamId, inValidTeamEdit)  as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Team does not exist", result.Value);
            _repository.Verify();
        }
        [Fact]
        public async void EditTeam_CalledWithValidTeamDetails_ReturnsNoContent()
        {
            var validTeamEdit = new  TeamToCreateDto {
                Id = 1,
                Name = "Burnley FC",
                FanStrength = 5000
            };
            var invalidTeamId = 1;
            var teamFromRepo = _mapper.Map<Team>(validTeamEdit);
            _repository.Setup(rep => rep.GetTeam(invalidTeamId)).ReturnsAsync(teamFromRepo);
            _repository.Setup(rep => rep.SaveAllChangesAsync()).ReturnsAsync(true);

            var result =  await _teamsController.EditTeam(invalidTeamId, validTeamEdit);

            _repository.Verify(rep=> rep.SaveAllChangesAsync(), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

         [Fact]
        public async void DeleteTeam_CalledWithInValidId_ReturnsNotFound()
        {
            var invalidTeamId = 77;
            _repository.Setup(rep => rep.GetTeam(invalidTeamId)).ReturnsAsync((Team)null);

            var res = await _teamsController.DeleteTeam(invalidTeamId);
            
            _repository.Verify(rep => rep.GetTeam(invalidTeamId));
            Assert.IsType<NotFoundObjectResult>(res);
        }
         [Fact]
        public async void DeleteTeam_CalledWithValidId_ReturnsOkResult()
        {
            var validTeamId = 1;
             var validTeam = new  Team {
                Id = 1,
                Name = "Burnley FC",
                FanStrength = 5000
            };
            _repository.Setup(rep => rep.GetTeam(validTeamId)).ReturnsAsync(validTeam);
            _repository.Setup(rep => rep.SaveAllChangesAsync()).ReturnsAsync(true);

            var res = await _teamsController.DeleteTeam(validTeamId);

            _repository.Verify(rep => rep.GetTeam(validTeamId));
            Assert.IsType<OkResult>(res);
        }  

    }
}