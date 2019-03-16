using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Fixtures.API.Data;
using Fixtures.API.DTOS;
using Fixtures.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Fixtures.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repository, IMapper mapper, IConfiguration config)
        {
            _repository = repository;
            _mapper = mapper;
            _config = config;
        }
        // GET api/values
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] UserForLoginDto userForLoginDto)
        {
            if (String.IsNullOrEmpty(userForLoginDto.Username.Trim()) || String.IsNullOrEmpty(userForLoginDto.Password.Trim()))
                return BadRequest("Username or Password is Invalid");
            var userFromRepo = await _repository.LoginUser(userForLoginDto.Username.ToLower(), userForLoginDto.Password);
            if (userFromRepo == null)
                return Unauthorized();
            var userToReturn = _mapper.Map<UserToReturnDto>(userFromRepo);
            //generate token
            var claims = new Claim[]{
                new Claim(ClaimTypes.NameIdentifier, userToReturn.Id.ToString()),
                new Claim(ClaimTypes.Role, userToReturn.UserRole.ToString()),
                new Claim(ClaimTypes.Name, userToReturn.Username)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var signinCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor(){
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = signinCred
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new {
                user = userToReturn,
                token = tokenHandler.WriteToken(token)
            });
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody]UserForRegistrationDto userForRegistrationDto)
        {
            if (String.IsNullOrEmpty(userForRegistrationDto.Username.Trim()) || String.IsNullOrEmpty(userForRegistrationDto.Password.Trim()))
                return BadRequest("Invalid Registeration Details");
            userForRegistrationDto.Username = userForRegistrationDto.Username.ToLower();

            if (await _repository.UserExists(userForRegistrationDto.Username))
                return BadRequest("Username exists");
            var userToCreate = _mapper.Map<User>(userForRegistrationDto);
            var createdUser =await _repository.RegisterUser(userToCreate, userForRegistrationDto.Password);
            var userToReturn = _mapper.Map<UserToReturnDto>(createdUser);
            return CreatedAtRoute("GetUser", new { controller="Users", id = userToReturn.Id}, userToReturn);
        }

    }
}