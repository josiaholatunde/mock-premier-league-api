using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fixtures.API.Helpers;
using Fixtures.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Fixtures.API.Data
{
    public class PremierLeagueRepository : IPremierLeagueRepository
    {
        private readonly DataContext _context;

        public PremierLeagueRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add<T>(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove<T>(entity);
        }

        public async Task<Fixture> GetFixture(int fixtureId)
        {
            var fixture = await _context.Fixtures.FirstOrDefaultAsync(fx => fx.Id == fixtureId);
            if (fixture == null)
                return null;
            return fixture;
        }

        public async Task<IEnumerable<Fixture>> GetFixtures(UserParams userParams)
        {
            var fixtures = _context.Fixtures
                                    .Include(t => t.HomeTeam)
                                    .Include(t => t.AwayTeam)
                                    .AsQueryable();
            if (userParams.IsFixtureCompleted.HasValue) {
                if (userParams.IsFixtureCompleted.Value) {
                    fixtures = fixtures.Where(fx => DateTime.Now > fx.KickOffTime.AddMinutes(90) && fx.IsFixtureCompleted == true);
                } else {
                    fixtures = fixtures.Where(fx => DateTime.Now <= fx.KickOffTime.AddMinutes(90) && fx.IsFixtureCompleted == false);
                }
            }
            if (userParams.teamId.HasValue) {
                fixtures = fixtures.Where(fx => fx.HomeTeamId == userParams.teamId.Value || 
                fx.AwayTeamId == userParams.teamId.Value);
            }


            return await fixtures.ToListAsync();
        }
        public async Task<IEnumerable<Fixture>> GetFixturesForTeam(int teamId)
        {
            var fixtures = await _context.Fixtures
                                        .Where(fx => fx.HomeTeamId == teamId)
                                        .ToListAsync();
            return fixtures;
        }

        public async Task<Team> GetTeam(int teamId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(tm => tm.Id == teamId);
            if (team == null)
                return null;
            return team;
        }
        public async Task<Team> GetTeamByTeamName(string teamName)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(tm => tm.Name == teamName);
            if (team == null)
                return null;
            return team;
        }

        public async Task<IEnumerable<Team>> GetTeams()
        {
            var teams = await _context.Teams.ToListAsync();
            return teams;
        }

        public async Task<User> GetUser(int userId)
        {
            var userFromRepo = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (userFromRepo == null)
                return null;
            return userFromRepo;
        }

        public Task<bool> MakeUserAdmin(User user)
        {
            return null;
        }

        public async Task<bool> SaveAllChangesAsync()
        {
           if (await _context.SaveChangesAsync() > 0)
                return true;
           return false;
        }

        public async Task<bool> TeamExists(string name)
        {
            if (await _context.Teams.AnyAsync(u => u.Name.ToLower() == name))
                return true;
            return false;
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async void DeleteChildren(int id)
        {
            var fixtures = await _context.Fixtures.Where(f => f.AwayTeamId == id || f.HomeTeamId == id).ToListAsync();
            _context.RemoveRange(fixtures);
        }

        
    }
}