using blogApp_DAL.Intarface;
using blogApp_DAL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace blogApp_DAL.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly Blog_DBcontext _context;

        public UserRepo(Blog_DBcontext context) 
        {
            _context = context;
        }

        public async Task<bool> CanLoginInAccount(string email, string password)
        {
            var user = await FindUserByEmail(email);
            if (user == null)
                return false;

            //if(user.Password != password)
            //    return false;

            return true;
        }


        public async Task UpdateUserAsync(User user)
        {
            _context.User.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task CreateUserAsync(User user)
        {
            var entry = _context.Entry(user);
            if (entry.State == EntityState.Detached)
                await _context.User.AddAsync(user);

            await _context.SaveChangesAsync();
        }

        public async Task<User> FindUserByEmail(string email)
        {
            return await _context.User.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User> FindUserById(string id)
        {
            return await _context.User.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
