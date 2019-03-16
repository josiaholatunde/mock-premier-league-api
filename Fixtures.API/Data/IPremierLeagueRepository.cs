using System.Collections.Generic;
using System.Threading.Tasks;
using Fixtures.API.Helpers;
using Fixtures.API.Models;

namespace Fixtures.API.Data
{
    public interface IPremierLeagueRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T: class;
        void Update<T>(T entity) where T: class;
        Task<bool> SaveAllChangesAsync();
        Task<IEnumerable<Fixture>> GetFixtures(UserParams userParams);
        Task<IEnumerable<Team>> GetTeams();
        Task<Team> GetTeam(int teamId);
        Task<Team> GetTeamByTeamName(string teamName);
        void DeleteChildren(int id);
        Task<Fixture> GetFixture(int fixtureId);
        Task<IEnumerable<Fixture>> GetFixturesForTeam(int teamId);
        Task<bool> TeamExists(string username);

        Task<User> GetUser(int userId);
        Task<bool> MakeUserAdmin(User user);

    }
}