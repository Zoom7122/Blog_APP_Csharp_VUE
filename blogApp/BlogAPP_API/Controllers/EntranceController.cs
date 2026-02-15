
using BlogAPP_BLL.Intarface;
using BlogAPP_BLL.Services;
using BlogAPP_Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogAPP_API.Controllers
{

    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class EntranceController : ControllerBase
    {
        private readonly ILoginService _logService;

        public EntranceController(ILoginService logService, IArticleService articleService) 
        {
            _logService = logService;
        }

        [HttpGet("boom")]
        public IActionResult Boom() => throw new Exception("test error");

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<BlogAPP_Core.Models.AuthResponseDto>> Login([FromBody] LoginDate data)
        {
            try
            {
                var user = await _logService.Login(data);
                if (user == null)
                    return Ok(new BlogAPP_Core.Models.AuthResponseDto
                    {
                        Success = false,
                        ErrorMessage = "Неверный Email или пароль"
                    });

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("Avatar", user.Avatar_url ?? ""),
                new Claim("Role", user.Role ?? "User"),
                new Claim("CountPost", user.CountPost.ToString() ?? "1"),
            };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

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

                return Ok(new BlogAPP_Core.Models.AuthResponseDto { Success = true, User = user });
            }
            catch (Exception ex)
            {
                return Ok(new BlogAPP_Core.Models.AuthResponseDto { Success = false, ErrorMessage = $"Ошибка: {ex.Message}" });
            }
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { success = true });
        }

        [HttpGet]
        [Route("CheckAuth")]
        public IActionResult CheckAuth()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = new
                {
                    Email = User.FindFirst(ClaimTypes.Email)?.Value,
                    Name = User.FindFirst(ClaimTypes.Name)?.Value,
                    Avatar = User.FindFirst("Avatar")?.Value,
                    Role = User.FindFirst("Role")?.Value,
                    CountPost = User.FindFirst("CountPost")?.Value
                };
                return Ok(new { success = true, user });
            }
            return Ok(new { success = false });
        }


        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto data)
        {
            try
            {
                var result = await _logService.Register(data);

                if (result)
                    return Ok(new { success = true });
                return Ok(new { success = false });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    messegeEror = ex.Message
                });
            }
        }
    }
}
