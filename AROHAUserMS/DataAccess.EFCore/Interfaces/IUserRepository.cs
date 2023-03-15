using AROHAUserMS.DataAccess.EFCore.Models;
using AROHAUserMS.Domain.Entities;


namespace AROHAUserMS.DataAccess.EFCore.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUser(int id);
        Task<User> GetUserByEmailAsync (string email,string password);
        Task<User> AddUser(User user);

        Task<User> AddUser(CreateUserModel createUser);
        Task<int?> DeleteUser(int id);
        Task<int?> UpdateUser(int id,User user);
    }
}
