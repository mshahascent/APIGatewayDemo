using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AROHAUserMS.DataAccess.EFCore;
using AROHAUserMS.Domain.Entities;
using AROHAUserMS.DataAccess.EFCore.Interfaces;
using AROHAUserMS.DataAccess.EFCore.Repositories;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using AROHAUserMS.DataAccess.EFCore.Models;

namespace AuthMicroservice.Controllers
{
    [Route("api/oldusers")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ArohaContext _context;
        private readonly IConfiguration _config;

        private readonly string reasonUserNotFound = "User Not Found";
        public IUserRepository Users { get; private set; }
        public UsersController(ArohaContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
            Users = new UserRepository(_context);
        }

        // GET: api/Users

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await Users.GetAllAsync().ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await Users.GetUser(id);

            if (user == null)
            {
                return BadRequest(reasonUserNotFound);
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest(reasonUserNotFound);
            }
            int? result = await Users.UpdateUser(id, user);
            if (result == null)
            {
                return BadRequest(reasonUserNotFound);
            }

            return NoContent();
        }

        //POST: api/Users
        //To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] CreateUserModel user)
        {
            if (user == null)
            {
                return BadRequest(reasonUserNotFound);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result =await Users.AddUser(user);

            return result;
        }

        //// DELETE: api/Users/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUser(int id)
        //{
        //    var user = await _context.Users.FindAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Users.Remove(user);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            int? result = await Users.DeleteUser(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPost]
        [Route("oldlogin", Name = "Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await Users.GetUserByEmailAsync(email, password);
            if (user == null)
            {
                return BadRequest(reasonUserNotFound);
            }
            var claims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.Iss,"AROHAAuth"),
                    new Claim(JwtRegisteredClaimNames.Sub, Convert.ToString(user.Id)),
                    new Claim(JwtRegisteredClaimNames.Aud, "Microservice"),
                    new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString(), "DateTime"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserID", Convert.ToString(user.Id)),
                    };

            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var secretBytes = Encoding.UTF8.GetBytes(Convert.ToString(_config["APISecretKey"]));
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;
            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(null, null, claims, DateTime.Now, DateTime.Now.AddMinutes(60), signingCredentials);

            var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(tokenJson);
        }
    }
}
