using System.Collections.Generic;
using System.Linq;
using Fixtures.API.Models;
using Newtonsoft.Json;

namespace Fixtures.API.Data
{
    public class SeedData
    {
         private readonly DataContext _context;

        public SeedData(DataContext context)
        {
            _context = context;
        }

        public void SeedUsers()
        {
           if (!_context.Users.Any())
           {
                var userData = System.IO.File.ReadAllText("Data/SeedUsersData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach (var user in users)
                {
                    byte[] passwordHash, passwordSalt;
                    ComputePasswordHash("password", out passwordHash, out passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.Username = user.Username.ToLower();
                    _context.Users.Add(user);
                }
                _context.SaveChanges();
           }
        }

        private void ComputePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        public void SeedTeams()
        {
            if (!_context.Teams.Any())
            {
                var teamData = System.IO.File.ReadAllText("Data/TeamSeed.json");
                var teams = JsonConvert.DeserializeObject<List<Team>>(teamData);
                foreach (var team in teams)
                {
                    _context.Add(team);
                    _context.SaveChanges();
                }
            }
        }
        public void SeedFixtures()
        {
            if (!_context.Fixtures.Any())
            {
                var fixturesData = System.IO.File.ReadAllText("Data/FixturesSeed.json");
                var fixtures = JsonConvert.DeserializeObject<List<Fixture>>(fixturesData);
                foreach (var fixture in fixtures)
                {
                    _context.Add(fixture);
                    _context.SaveChanges();
                }
            }
        }
    }
}