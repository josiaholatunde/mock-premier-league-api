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
    public class TeamsController : ControllerBase
    {
        private readonly IPremierLeagueRepository _repository;
        private readonly IMapper _mapper;

        public TeamsController(IPremierLeagueRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        // GET api/values
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
          var teams = await _repository.GetTeams();
          var teamsToReturn = _mapper.Map<IEnumerable<TeamToReturnDto>>(teams);
          return Ok(teamsToReturn);
        }

        // GET api/values/5
        [HttpGet("{id}", Name="GetTeam")]
        public async Task<IActionResult> GetTeam(int id)
        {
            var team = await _repository.GetTeam(id);
            if (team == null)
                return BadRequest("Team does not exist");
            var teamToReturn = _mapper.Map<TeamToReturnDto>(team);
            return Ok(teamToReturn);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] TeamToCreateDto teamToCreateDto)
        {
            if (await _repository.TeamExists(teamToCreateDto.Name))
                return BadRequest("Team name exists");
            var teamToCreate = _mapper.Map<Team>(teamToCreateDto);
            _repository.Add<Team>(teamToCreate);
            if (await _repository.SaveAllChangesAsync())
            {
                var teamToReturn = _mapper.Map<TeamToReturnDto>(teamToCreate);
                return CreatedAtRoute("GetTeam", new { id = teamToReturn.Id, name=teamToReturn.Name}, teamToReturn);
            }
            return BadRequest("An Error occurred while creating Team");

        }
        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditTeam(int id, [FromBody] TeamToCreateDto teamToEditDto)
        {
            if (id != teamToEditDto.Id || teamToEditDto.Id == 0)
                return BadRequest("Team id does not match id on body of request");
            var teamFromRepo = await _repository.GetTeam(id);
            if (teamFromRepo == null)
                return NotFound("Team does not exist");
            var teamToUpdate = _mapper.Map<TeamToCreateDto, Team>(teamToEditDto, teamFromRepo);
            _repository.Update<Team>(teamToUpdate);
            if (await _repository.SaveAllChangesAsync())
            {
                var teamToReturn = _mapper.Map<TeamToReturnDto>(teamToUpdate);
                return NoContent();
            }
            throw new Exception($"Error Editing team with id {id}");
        }

         [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var teamFromRepo = await _repository.GetTeam(id);
            if (teamFromRepo == null)
                return NotFound("Team does not exist");
            _repository.Delete<Team>(teamFromRepo);
            _repository.DeleteChildren(id);
            if (await _repository.SaveAllChangesAsync())
            {
                return Ok();
            }
            return BadRequest($"An Error occurred while deleting team with id {id}");
        }
    
    }
}
