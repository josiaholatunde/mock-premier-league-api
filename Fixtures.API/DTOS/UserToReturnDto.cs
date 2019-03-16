using Fixtures.API.Models;

namespace Fixtures.API.DTOS
{
    public class UserToReturnDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public UserRole UserRole { get; set; }

        public string FavoriteTeam { get; set; }
    }
}