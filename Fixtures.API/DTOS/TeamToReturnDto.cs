using System;
using System.Collections.Generic;
using Fixtures.API.Models;

namespace Fixtures.API.DTOS
{
    public class TeamToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LocationCity { get; set; }
        public string LocationCountry { get; set; }
        public double FanStrength { get; set; }
        public string ShortName { get; set; }
        public int Win { get; set; }
        public int Loss { get; set; }
        public int Draw { get; set; }
        public int Points { get; set; }
        public string StadiumName { get; set; }
        public string NameOfManager { get; set; }
        public DateTime YearFounded { get; set; }
    }
}