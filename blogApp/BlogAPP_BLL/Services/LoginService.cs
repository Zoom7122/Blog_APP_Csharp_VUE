using AutoMapper;
using BlogAPP_BLL.Exceptions;
using BlogAPP_BLL.Intarface;
using BlogAPP_BLL.Models;
using BlogAPP_Core.Models;
using blogApp_DAL.Intarface;
using blogApp_DAL.Model;
using System;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlogAPP_BLL.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        private readonly IArticleService _articleServic;
        private readonly IPasswordService _passwordService;

        public LoginService(IUserRepo userRepo, IMapper mapper, IArticleService articleService,
            IPasswordService passwordService)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _articleServic = articleService;
            _passwordService = passwordService;
        }
        public async Task<UserEnrance> UpdateUserAsync(string currentEmail, BlogAPP_Core.Models.UpdateUserDto data)
        {
            if (string.IsNullOrWhiteSpace(currentEmail))
                throw new ArgumentException("Текущий email не может быть пустым", nameof(currentEmail));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var user = await _userRepo.FindUserByEmail(currentEmail);
            if (user == null)
                throw new UserNotFoundException("Пользователь не найден");

            var normalizedEmail = string.IsNullOrWhiteSpace(data.Email) ? user.Email : data.Email.Trim();

            if (!string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
            {
                var existingUser = await _userRepo.FindUserByEmail(normalizedEmail);
                if (existingUser != null)
                    throw new RegisterException("Пользователь с таким email уже существует");
            }

            user.FirstName = data.FirstName?.Trim() ?? user.FirstName;
            user.Email = normalizedEmail;
            user.Avatar_url = data.Avatar_url?.Trim() ?? string.Empty;
            user.Bio = data.Bio?.Trim() ?? string.Empty;

            await _userRepo.UpdateUserAsync(user);

            var infoUser = _mapper.Map<UserEnrance>(user);
            infoUser.CountPost = await _articleServic.CountArticleWroteByUserAsync(user.Email);

            return infoUser;
        }

        public async Task<UserEnrance> Login(LoginDate data)
        {

            var user = await _userRepo.FindUserByEmail(data.Email);

            if (user == null || !_passwordService.VerifyPassword(data.Password, user.Password))
            {
                throw new Exception("Не правильный пароль");
            }

            var infoUser = _mapper.Map<UserEnrance>(user);

            var countPost = await _articleServic.CountArticleWroteByUserAsync(user.Email);

            infoUser.CountPost = countPost;

            return infoUser;
        }

        public async Task<User> FindUserByEmail(string email)
        {
           return await _userRepo.FindUserByEmail(email);
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

            var passwordHash = _passwordService.HashPassword(user.Password);
            user.Password = passwordHash;

            await _userRepo.CreateUserAsync(user);

            return true;
        }
    }
}