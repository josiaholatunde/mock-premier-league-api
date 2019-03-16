using System;
using System.ComponentModel.DataAnnotations;

namespace Fixtures.API.DTOS
{
    public class UserForRegistrationDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [MinLength(7, ErrorMessage="Password must not contain less than 7 characters"), MaxLength(15, 
        ErrorMessage="Password must not contain more than 15 characters")]
        public string Password { get; set; }
        [Required]
        public string FavoriteTeam { get; set; }

    }
}