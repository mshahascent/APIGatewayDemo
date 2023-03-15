using AROHAUserMS.DataAccess.EFCore.Models;
using AROHAUserMS.DataAccess.EFCore.Interfaces;
using AROHAUserMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace AROHAUserMS.DataAccess.EFCore.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        protected readonly ArohaContext _context;
        public UserRepository(ArohaContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await GetAllAsync()
                            .OrderBy(x => x.Firstname)
                            .ToListAsync();
        }
        public async  Task<User> GetUser(int id)
        {
           return await FindByCondition(user => user.Id.Equals(id)).FirstOrDefaultAsync(); 
        }
        public async Task<User> GetUserByEmailAsync(string email, string password)
        {
            return await FindByCondition(user => user.Email.Equals(email)
                        && user.Password.Trim().Equals(password.Trim())).SingleOrDefaultAsync();
        }
        public async Task<User> AddUser(User u)
        {
            this.Add(u);
            int userID = _context.SaveChanges();
            return await GetUser(userID);
        }
        public async Task<User> AddUser(CreateUserModel userModel)
        {
            User user = new User();
            user.Email = userModel.Email;
            user.Password = userModel.Password;
            this.Add(user);
            int userID = _context.SaveChanges();
            return await GetUser(userID);
        }
        public async Task<int?> DeleteUser(int id)
        {
            User? user = this.Find(ur => ur.Id == id).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            this.Remove(user);
            return await _context.SaveChangesAsync();
        }
        public async Task<int?> UpdateUser(int id,User newUserInfo)
        {

            User? existingUser = this.Find(ur => ur.Id == id).FirstOrDefault();
            if (existingUser == null)
            {
                return null;
            }
            else
            {
                _context.Entry(newUserInfo).State = EntityState.Modified;
                existingUser.Firstname = newUserInfo.Firstname;
                existingUser.Middlename=newUserInfo.Middlename;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
                    {
                        return null;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            //this.Remove(newUserInfo);

            return await _context.SaveChangesAsync();
        }
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
