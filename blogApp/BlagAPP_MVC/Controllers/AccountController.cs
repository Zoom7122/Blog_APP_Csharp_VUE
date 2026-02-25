using BlagAPP_MVC.Models;
using BlogAPP_BLL.Intarface;
using BlogAPP_Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlagAPP_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILoginService loginService, ILogger<AccountController> logger)
        {
            _logger = logger;
            _loginService = loginService;
        }
        
        /// <summary>
        /// Для проверки страницы "Что-то пошло не так"
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("BOOM")]
        public async Task<IActionResult> boom()
        {
            return StatusCode(500);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            try
            {
                _logger.LogInformation($"Login attempt for { model.Email}");
                var user = await _loginService.Login(new LoginDate
                {
                    Email = model.Email,
                    Password = model.Password
                });

                var userRole = string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role;

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Name, user.Name),
                    new("Avatar", user.Avatar_url ?? string.Empty),
                    new("Role", userRole),
                    new(ClaimTypes.Role, userRole),
                    new("CountPost", user.CountPost.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    _logger.LogInformation($"User {user.Email} logged in and redirected to returnUrl");
                    return Redirect(returnUrl);
                }

                _logger.LogInformation($"User {user.Email} logged in successfully");

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation($"User {email} logged out");

            return RedirectToAction("Login", "Account");
        }



        [AllowAnonymous]
        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View(new RegisterViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _logger.LogInformation($"Register attempt for {model.Email}");

                await _loginService.Register(new CreateUserDto
                {
                    FirstName = model.FirstName.Trim(),
                    Email = model.Email.Trim(),
                    Password = model.Password,
                    Avatar_url = model.AvatarUrl?.Trim(),
                    Bio = model.Bio?.Trim(),
                    Role = "User"
                });

                TempData["SuccessMessage"] = "Регистрация прошла успешно. Теперь войдите в аккаунт.";
                _logger.LogInformation($"User {model.Email} registered successfully");
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Register failed for {model.Email}");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }

            
        }

    }
}
