using AROHAUserMS.DataAccess.EFCore.Models;
using AuthMicroservice.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DataAccess.EFCore.Repositories
{
    public interface ICIndentityUserRepository
    {
        Task<IdentityResult> Register(CreateUserModel user);
        Task<TokenModel> Login(AuthenticateUserModel authUser);

        Task<IdentityResult> Delete(string id);
    }
}