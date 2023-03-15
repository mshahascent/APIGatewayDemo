using AuthDataAccess.Entities;
using AuthMicroservice.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using AROHAUserMS.DataAccess.EFCore.Models;

namespace AuthMicroservice.Profiles
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<CreateRoleModel, IdentityRole>();
            CreateMap<UpdateRoleModel, IdentityRole>();
            CreateMap<IdentityRole, RoleModel>().ForMember(d => d.RoleName, m => m.MapFrom(s => s.Name));
            CreateMap<CreateUserModel, CIdentityUser>().ForMember(d => d.UserName, m => m.MapFrom(s => s.Email));
            CreateMap<CreateTUserModel, CIdentityUser>().ForMember(d => d.UserName, m => m.MapFrom(s => s.Email));
        }

    }
}
