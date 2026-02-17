using BlogAPP_BLL.Models;
using BlogAPP_Core.Models;
using blogApp_DAL.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogAPP_BLL.Intarface
{
    public interface ILoginService
    {
        Task<UserEnrance> Login(LoginDate data);

        Task<bool> Register(CreateUserDto data);

        Task<User> FindUserByEmail(string email);

        Task<UserEnrance> UpdateUserAsync(string currentEmail, BlogAPP_Core.Models.UpdateUserDto data);
    }
}
