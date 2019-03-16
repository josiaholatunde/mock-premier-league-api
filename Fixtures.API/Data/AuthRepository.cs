using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fixtures.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Fixtures.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> LoginUser(string username, string password)
        {
            var userFromRepo = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (userFromRepo == null)
                return null;
            if (!VerifyPasswordHash(password, userFromRepo.PasswordHash, userFromRepo.PasswordSalt))
                return null;
            return userFromRepo;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
             using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedPasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedPasswordHash.Length; i++)
                {
                    if (passwordHash[i] != computedPasswordHash[i])
                        return false;
                }
                return true;
            }
        }

        public async Task<User> RegisterUser(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            ComputePasswordHash(password, out passwordHash, out passwordSalt);
            user.Username = user.Username.ToLower();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _context.Add<User>(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private void ComputePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
                return true;
            return false;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }
    }
}