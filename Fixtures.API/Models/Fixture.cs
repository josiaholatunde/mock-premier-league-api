using System;

namespace Fixtures.API.Models
{
    public class Fixture
    {
        public int Id { get; set; }
        public int? HomeTeamScore { get; set; }
        public Team HomeTeam { get; set; }
        public int HomeTeamId { get; set; }
        public Team AwayTeam { get; set; }
        public int AwayTeamId { get; set; }

        public int AwayTeamScore { get; set; }

        public DateTime KickOffTime { get; set; }
        public string LocationOfPlay { get; set; }
        public bool IsFixtureCompleted { get; set; }
        
    }
}