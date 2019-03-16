using System;
using System.ComponentModel.DataAnnotations;

namespace Fixtures.API.DTOS
{
    public class FixtureToCreate
    {
        public int Id { get; set; }
        [Required]
        public int HomeTeamId { get; set; }
        public int HomeTeamScore { get; set; }
         [Required]
        public int AwayTeamId { get; set; }

        public int AwayTeamScore { get; set; }
         [Required]

        public DateTime KickOffTime { get; set; }
        [Required]
        public string  LocationOfPlay { get; set; }
        public bool IsFixtureCompleted { get; set; }
    }
}