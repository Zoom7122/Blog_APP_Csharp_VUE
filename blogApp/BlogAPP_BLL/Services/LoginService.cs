using AutoMapper;
using BlogAPP_BLL.Exceptions;
using BlogAPP_BLL.Intarface;
using BlogAPP_Core.Models;
using blogApp_DAL.Intarface;
using blogApp_DAL.Model;
using System;
using System.Threading.Tasks;

namespace BlogAPP_BLL.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        private readonly IArticleService _articleServic;

        public LoginService(IUserRepo userRepo, IMapper mapper, IArticleService articleService)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _articleServic = articleService;
        }

        public async Task<UserEnrance> Login(LoginDate data)
        {
            var result = await _userRepo.CanLoginInAccount(data.Email, data.Password);

            if (!result)
            {
                return null;
            }

            var user = await _userRepo.FindUserByEmail(data.Email);

            var infoUser = _mapper.Map<UserEnrance>(user);

            var countPost = await _articleServic.CountArticleWroteByUserAsync(user.Email);

            infoUser.CountPost = countPost;

            return infoUser;
        }

        public async Task<bool> Register(CreateUserDto data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "Данные регистрации не могут быть null");

            if (string.IsNullOrWhiteSpace(data.Password))
                throw new RegisterException("Пароль не может быть пустым");

            if (data.Password.Length < 8)
                throw new RegisterException("Пароль должен содержать минимум 8 символов");

            var existingUser = await _userRepo.FindUserByEmail(data.Email);
            if (existingUser != null)
                throw new RegisterException("Пользователь с таким email уже существует");

            var user = _mapper.Map<User>(data);

            if (string.IsNullOrWhiteSpace(user.Role))
            {
                user.Role = "User";
            }

            _userRepo.CreateUserAsync(user);

            return true;
        }
    }
}