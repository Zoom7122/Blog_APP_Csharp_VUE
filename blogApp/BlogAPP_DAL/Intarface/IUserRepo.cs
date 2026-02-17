using blogApp_DAL.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace blogApp_DAL.Intarface
{
    public interface IUserRepo
    {
        Task<User> FindUserByEmail(string email);

        Task<bool> CanLoginInAccount(string email, string password);

        Task CreateUserAsync(User user);

        Task<User> FindUserById(string id);


        Task UpdateUserAsync(User user);
    }
}
