using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDTO userForRegisterDTO)
        {
            userForRegisterDTO.UserName = userForRegisterDTO.UserName.ToLower();
            if (await _repo.UserExists(userForRegisterDTO.UserName))
                return BadRequest("Username is already exists");

            var userToCreate = new User
            {
                Username = userForRegisterDTO.UserName
            };
            var createdUser = _repo.Register(userToCreate, userForRegisterDTO.Password);
            return StatusCode(201);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserForLoginDTO userForLoginDTO)
        {
            var userFromRepo = await _repo.login(userForLoginDTO.UserName.ToLower()
                , userForLoginDTO.Password);
            if (userFromRepo == null)
                return Unauthorized();
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };
            var key = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            
            var creds= new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials =creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}