using BlogAPP_BLL.Intarface;
using BlogAPP_BLL.Models;
using BlogAPP_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime;
using System.Security.Claims;

namespace BlogAPP_API.Controllers
{
    [Authorize]
    //[AllowAnonymous]
    [Route("api/[controller]")]
    public class ArticleConrtroller : ControllerBase
    {
        private readonly IArticleService _articleService;
        public ArticleConrtroller(IArticleService articleService) 
        {
            _articleService = articleService;
        }

        [HttpPost]
        [Route("CreateArticle")]
        public async Task<IActionResult> CreateArticleAsync([FromBody] CreateArticleModel model)
        {
            try
            {
                var allClimes  = User.Claims.ToList();

                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                var role = User.FindFirst("Role")?.Value;
                var avatar = User.FindFirst("Avatar")?.Value;

                UserCookie userCookie = new UserCookie()
                {
                    Email = email,
                    Name = name,
                    Role = role,
                    Avatar = avatar
                };

                var result = await _articleService.CreateArticle(model, userCookie);

                if (result)
                    return Ok(new { success = true });
                else return Ok(new { success = false });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false,
                    messegeError = ex.Message });
            }
        }
    }
}
