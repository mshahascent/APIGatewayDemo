using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AROHAUserMS.DataAccess.EFCore;
using AROHAUserMS.Domain.Entities;
using AROHAUserMS.DataAccess.EFCore.Repositories;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using AROHAUserMS.DataAccess.EFCore.Interfaces;

namespace AROHAUserMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ArohaContext _context;

        private readonly string reasonUserNotFound = "User Not Found";
        public IUserRepository Users { get; private set; }
        public UsersController(ArohaContext context)
        {
            _context = context;
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
            int? result=await Users.UpdateUser(id, user);
            if (result == null)
            {
                return BadRequest(reasonUserNotFound);
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (user == null)
            {
                return  BadRequest(reasonUserNotFound);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await Users.AddUser(user);
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
            int? result=await Users.DeleteUser(id);
            if (result == null)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpPost]
        [Route("login", Name = "Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(int id)
        {
            var user = await Users.GetUser(id);
            if (user == null)
            {
                return BadRequest(reasonUserNotFound);
            }
            var claims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.Iss,"AROHAAuth"),
                    new Claim(JwtRegisteredClaimNames.Sub, Convert.ToString(user.Id)),
                    new Claim(JwtRegisteredClaimNames.Aud, Convert.ToString(user.Id)),
                    //new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddMinutes(10)).ToUnixTimeSeconds().ToString()),
                    // new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString(), "DateTime"),
                    new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString(), "DateTime"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserID", Convert.ToString(user.Id)),
                    };

            //foreach (var userRole in userRoles)
            //{
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            //var role = await _roleManager.FindByNameAsync(userRole);
            //if (role != null)
            //{
            //    var roleClaims = await _roleManager.GetClaimsAsync(role);
            //    foreach (Claim roleClaim in roleClaims)
            //    {
            //        claims.Add(roleClaim);
            //    }
            //}
            // }

            var secretBytes = Encoding.UTF8.GetBytes("sometstxvxxdgdfgfgfgdfgdfgdfgdf");
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;
            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(null, null, claims, DateTime.Now, DateTime.Now.AddMinutes(10), signingCredentials);

            var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(tokenJson);
        }
    }
}
