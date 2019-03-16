using System;
using System.ComponentModel.DataAnnotations;

namespace Fixtures.API.DTOS
{
    public class TeamToCreateDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public LocationDto Location { get; set; }
        [Required]
        public double FanStrength { get; set; }
        [Required]
        [MaxLength(3)]
        public string ShortName { get; set; }
        [Required]
        public string StadiumName { get; set; }
        [Required]
        public string NameOfManager { get; set; }
        [Required]
        public DateTime YearFounded { get; set; }
        public string LogoUrl { get; set; }
    }
}