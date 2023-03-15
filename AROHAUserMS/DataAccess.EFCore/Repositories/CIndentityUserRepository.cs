using AROHAUserMS.DataAccess.EFCore.Models;
using AuthDataAccess.Entities;
using AutoMapper;
using DataAccess.EFCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AROHAUserMS.DataAccess.EFCore.Services;
using AuthMicroservice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AROHAUserMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

namespace DataAccess.EFCore.Repositories
{
    public class CIndentityUserRepository : ICIndentityUserRepository
    {
        UserManager<CIdentityUser> _userManager;
        SignInManager<CIdentityUser> _signInManager;
        IMapper _mapper;
        IEmailSender _emailSender;
        private readonly IConfiguration _config;
        ILogger<CIndentityUserRepository> _log;
        public CIndentityUserRepository(UserManager<CIdentityUser> userManager, IMapper mapper,
            SignInManager<CIdentityUser> signInManager, IEmailSender emailSender, IConfiguration configuration)
        {
            _userManager = userManager;
            _mapper = mapper;
            _emailSender = emailSender;
            this._config = configuration;
            _signInManager = signInManager;
            _emailSender = emailSender;
            //_log = log;
            //  Options = optionsAccessor.Value;
        }
        public async Task<IdentityResult> Register( CreateUserModel user)
        {
            return await CommonRegister(user);
        }
        private async Task<IdentityResult> CommonRegister<T>( T user) where T : CreateUserModel
        {
            CIdentityUser CIdentityUser = _mapper.Map<CIdentityUser>(user);
            IdentityResult result;
       
            result = await _userManager.CreateAsync(CIdentityUser, user.Password);

            if (result.Succeeded)
            {
                try
                {
                    var roleResult = await _userManager.AddToRolesAsync(CIdentityUser, new string[] { user.Role});
                    if (roleResult.Succeeded)
                    {
                        // Create token
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(CIdentityUser);
                        //user.ConfirmUrl = "www.abc.com";
                        //var callbackUrl = user.ConfirmUrl + "?token=" + HttpUtility.UrlEncode(token) + "&userid=" + CIdentityUser.Id;

                        //Send email for Confirmation
                        //EmailMessage email = new EmailMessage(CIdentityUser.Email, "Account Confirm Email", "Call back URL -> " + callbackUrl + "");
                        //await _emailSender.SendEmailAsync(email);
                       return result;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            //foreach (var error in result.Errors)
            //{
            //    _log.LogError(error.Description, error);
            //}

            return result;
        }

        public async Task<TokenModel> Login(AuthenticateUserModel authUser)
        {
            TokenModel _token = null;
            var user = await _userManager.FindByEmailAsync(authUser.Email);

            //if (user == null)
            //{
            //    return  BadRequest("User not found");
            //}

            //if (user != null && !user.EmailConfirmed && await _userManager.CheckPasswordAsync(user, authUser.Password))
            //{
            //    return BadRequest("Email not confirmed yet");
            //}

            var result = await _signInManager.PasswordSignInAsync(authUser.Email, authUser.Password, false, false);

            if (result.Succeeded)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

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
                _token = new TokenModel();
               _token.Token = tokenJson;
            }
            return _token;
            //return StatusCode(StatusCodes.Status500InternalServerError, "Couldn't login");
        }
        public async Task<IdentityResult> Delete(string id)
        {
            
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                return await _userManager.DeleteAsync(user);
            }

            return null;
        
    
        }
    }
}
