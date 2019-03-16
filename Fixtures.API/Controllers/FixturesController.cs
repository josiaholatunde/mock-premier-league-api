using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fixtures.API.Data;
using Fixtures.API.DTOS;
using Fixtures.API.Helpers;
using Fixtures.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixtures.API.Controllers
{
    [Authorize(Roles= nameof(UserRole.Admin))]
    [Route("api/[controller]")]
    [ApiController]
    public class FixturesController : ControllerBase
    {
        private readonly IPremierLeagueRepository _repository;
        private readonly IMapper _mapper;

        public FixturesController(IPremierLeagueRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetFixtures([FromQuery] UserParams userParams)
        {
            var fixtures = await _repository.GetFixtures(userParams);
            var fixturesToReturn = _mapper.Map<IEnumerable<FixtureToReturn>>(fixtures);
            return Ok(fixturesToReturn);
        }

        [HttpGet("{id}", Name="GetFixture")]
        public async Task<IActionResult> GetFixture(int id)
        {
            var fixtureFromRepo = await _repository.GetFixture(id);
            if (fixtureFromRepo == null)
                return BadRequest("Fixture does not exist");
            var fixtureToReturn = _mapper.Map<FixtureToReturn>(fixtureFromRepo);
            return Ok(fixtureToReturn);
        }


        [HttpPost]
        public async Task<IActionResult> CreateTeamFixture([FromBody] FixtureToCreate fixtureToCreateDto)
        {
            var homeTeamFromRepo = await _repository.GetTeam(fixtureToCreateDto.HomeTeamId);
            if (homeTeamFromRepo == null)
                return BadRequest("Home Team specified does not exist");
            var awayTeamFromRepo = await _repository.GetTeam(fixtureToCreateDto.AwayTeamId);
            if (awayTeamFromRepo == null)
                return BadRequest("Away Team specified does not exist");
            if (DateTime.Now < fixtureToCreateDto.KickOffTime) {
                return BadRequest("You can not set the score for a match that has not begun");
            }
            if (DateTime.Now > fixtureToCreateDto.KickOffTime.AddMinutes(90))
                fixtureToCreateDto.IsFixtureCompleted = true;
            var fixtureToCreate = _mapper.Map<Fixture>(fixtureToCreateDto);
            _repository.Add(fixtureToCreate);
            if (await _repository.SaveAllChangesAsync())
            {
                var fixtureToReturn = _mapper.Map<FixtureToReturn>(fixtureToCreate);
                return CreatedAtRoute("GetFixture", new { 
                    id = fixtureToReturn.Id, 
                }, fixtureToReturn);
            }
            return BadRequest("An Error occurred while creating the fixture");
        }

        [HttpPut("{fixtureId}")]
        public async Task<IActionResult> EditFixture(int fixtureId, [FromBody] FixtureToCreate fixtureToEditDto)
        {
            if (fixtureId != fixtureToEditDto.Id)
                return BadRequest($"Fixture Id on the route {fixtureId} does not match fixtureId on the body of request {fixtureToEditDto.Id}");
            var fixtureFromRepo = await _repository.GetFixture(fixtureId);
            if (fixtureFromRepo == null)
                return BadRequest($"Fixture with id {fixtureId} does not exist");

            var homeTeamFromRepo = await _repository.GetTeam(fixtureToEditDto.HomeTeamId);
            if (homeTeamFromRepo == null)
                return BadRequest($"Home Team Id {fixtureToEditDto.Id} does not exist");

            var awayTeamFromRepo = await _repository.GetTeam(fixtureToEditDto.HomeTeamId);
            if (awayTeamFromRepo == null)
                return BadRequest($"Away Team  Id {fixtureToEditDto.Id} does not exist");
            
            
            var fixtureToUpdate = _mapper.Map<FixtureToCreate, Fixture>(fixtureToEditDto, fixtureFromRepo);
            _repository.Update<Fixture>(fixtureToUpdate);
            if (await _repository.SaveAllChangesAsync())
            {
                var fixtureToReturn = _mapper.Map<FixtureToReturn>(fixtureToUpdate);
                return NoContent();
            }
            throw new Exception($"Error Editing fixture with id {fixtureToEditDto.Id}");
        }
    }
}