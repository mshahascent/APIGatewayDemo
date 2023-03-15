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
using DataAccess.EFCore.DBContext;
using DataAccess.EFCore.Repositories;
using AuthDataAccess.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using AROHAUserMS.DataAccess.EFCore.Services;
using AuthMicroservice.Models;

namespace AuthMicroservice.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class CIdentityUsersController : ControllerBase
    {
        //private readonly AuthManagementDbContext _context;
        private readonly IConfiguration _config;
        UserManager<CIdentityUser> _userManager;
        SignInManager<CIdentityUser> _signInManager;
        IMapper _mapper;
        IEmailSender _emailSender;
        ILogger<CIdentityUsersController> _log;

        private readonly string reasonUserNotFound = "User Not Found";
        public ICIndentityUserRepository Users { get; private set; }

        public CIdentityUsersController(UserManager<CIdentityUser> userManager, IMapper mapper,
            SignInManager<CIdentityUser> signInManager, IEmailSender emailSender,IConfiguration configuration)
        {
            // _context = context;
            _emailSender = emailSender;
            this._config = configuration;
            Users = new CIndentityUserRepository(userManager, mapper, signInManager, _emailSender, _config);

        }


        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(CreateUserModel user)
        {
            var result = await Users.Register(user);
            if (result.Succeeded)
            {
                return Ok("User Created");
            }
            //foreach (var error in result.Errors)
            //{
            //    _log.LogError(error.Description, error);
            //}

            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
        }

        [HttpPost]
        [Route("IdentityLogin", Name = "IdentityLogin")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromForm] AuthenticateUserModel authUser)
        {
            var token=await Users.Login(authUser);
            if (token == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Couldn't login");
            }
            return Ok(token);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(string id)
        { 
            var result=await Users.Delete(id);
            if (result !=null && result.Succeeded)
            {
                return Ok("User deleted successfully");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Couldn't delete user");

        }

    }
}
