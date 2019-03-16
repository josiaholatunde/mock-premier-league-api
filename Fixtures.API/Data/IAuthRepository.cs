using System.Collections.Generic;
using System.Threading.Tasks;
using Fixtures.API.Models;

namespace Fixtures.API.Data
{
    public interface IAuthRepository
    {
         Task<User> LoginUser(string username, string password);
         Task<User> RegisterUser(User user, string password);
         Task<bool> UserExists(string username);
         Task<IEnumerable<User>> GetUsers();
    }
}