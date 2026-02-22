using BlagAPP_MVC.Models;
using BlogAPP_BLL.Intarface;
using BlogAPP_Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlagAPP_MVC.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ILoginService _loginService;
    private readonly IArticleService _articleService;
    private readonly ICommentsService _commentsService;

    public DashboardController(
        ILoginService loginService,
        IArticleService articleService,
        ICommentsService commentsService)
    {
        _loginService = loginService;
        _articleService = articleService;
        _commentsService = commentsService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = await BuildDashboardModelAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel updateProfile)
    {
        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildDashboardModelAsync(updateProfile);
            return View("Index", invalidModel);
        }

        var currentEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(currentEmail))
        {
            return RedirectToAction("Login", "Account");
        }

        var result = await _loginService.UpdateUserAsync(currentEmail, new UpdateUserDto
        {
            FirstName = updateProfile.FirstName,
            Email = updateProfile.Email,
            Avatar_url = updateProfile.AvatarUrl ?? string.Empty,
            Bio = updateProfile.Bio ?? string.Empty
        });

        await RefreshUserClaimsAsync(result);

        TempData["ProfileMessage"] = "Профиль успешно обновлен";
        return RedirectToAction(nameof(Index));
    }

    private async Task<DashboardViewModel> BuildDashboardModelAsync(UpdateProfileViewModel? updateForm = null)
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var user = await _loginService.FindUserByEmail(email);

        if (user == null)
        {
            return new DashboardViewModel();
        }

        var countPosts = await _articleService.CountArticleWroteByUserAsync(user.Email);
        var countComments = await _commentsService.GetCountCommentsWroteByUser(user.Id);

        return new DashboardViewModel
        {
            Name = user.FirstName,
            Email = user.Email,
            AvatarUrl = user.Avatar_url,
            Bio = user.Bio,
            Role = user.Role,
            CountPost = countPosts,
            CountCommentsUser = countComments,
            UpdateProfile = updateForm ?? new UpdateProfileViewModel
            {
                FirstName = user.FirstName,
                Email = user.Email,
                AvatarUrl = user.Avatar_url,
                Bio = user.Bio
            }
        };
    }

    private async Task RefreshUserClaimsAsync(UserEnrance user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name),
            new("Avatar", user.Avatar_url ?? string.Empty),
            new("Role", user.Role ?? "User"),
            new(ClaimTypes.Role, user.Role ?? "User"),
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
    }
}
