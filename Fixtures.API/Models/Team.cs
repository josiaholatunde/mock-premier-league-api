using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Fixtures.API.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LocationCity { get; set; }
        public string LocationCountry { get; set; }
        public string LogoUrl { get; set; }
        [MinLength(3)]
        public string ShortName { get; set; }
        public int Win { get; set; }
        public int Loss { get; set; }
        public int Draw { get; set; }
        public int Points { get; set; }

        public double FanStrength { get; set; }
        public string StadiumName { get; set; }
        public string NameOfManager { get; set; }
        public DateTime YearFounded { get; set; }
        public ICollection<Fixture> HomeFixtures { get; set; }
        public ICollection<Fixture> AwayFixtures { get; set; }
        public Team()
        {
            HomeFixtures = new Collection<Fixture>();
            AwayFixtures = new Collection<Fixture>();
        }

    }
}