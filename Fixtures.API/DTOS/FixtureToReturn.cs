using System;

namespace Fixtures.API.DTOS
{
    public class FixtureToReturn
    {
         public int Id { get; set; }
        public string HomeTeamName { get; set; }
        public int HomeTeamId { get; set; }
        private int homeTeamScore;
        public int? HomeTeamScore { 
            get {
                if(IsFixtureCompleted == false || DateTime.Now < KickOffTime) 
                    return null;
                return homeTeamScore;
            } 
            set {
                homeTeamScore = value.HasValue ? value.Value : 0;
            }
        }
        public string AwayTeamName { get; set; }
        public int AwayTeamId { get; set; }

        private int awayTeamScore = 0;

         public int? AwayTeamScore { 
            get {
                if(IsFixtureCompleted == false || DateTime.Now < KickOffTime) 
                    return null;
                return awayTeamScore;
            } 
             set {
                homeTeamScore = value.HasValue ? value.Value : 0;
            }
        }

        public DateTime KickOffTime { get; set; }
        public string  LocationOfPlay { get; set; }
        public bool IsFixtureCompleted { get; set; }
    }
}