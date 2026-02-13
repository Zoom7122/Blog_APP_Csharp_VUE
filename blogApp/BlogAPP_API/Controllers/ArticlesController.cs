using BlogAPP_BLL.Intarface;
using BlogAPP_BLL.Models;
using BlogAPP_Core.Models;
using blogApp_DAL.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime;
using System.Security.Claims;

namespace BlogAPP_API.Controllers
{

    [ApiController]
    [Authorize]
    //[AllowAnonymous]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;
        public ArticlesController(IArticleService articleService) 
        {
            _articleService = articleService;
        }

        [HttpPost]
        [Route("CreateArticle")]
        public async Task<IActionResult> CreateArticleAsync(
            [FromBody] CreateArticleModel model)
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

        [HttpPost]
        [Route("FindByProperties")]
        public async Task<IActionResult> FindByProperties(
          [FromBody] ArticlePropertiesFind propertiesFind)
        {
            try
            {
                List<ArticleReturnInAPI> list = await _articleService.FindArticleByProperties(propertiesFind);

                if (list == null || list.Count <= 0)
                    return Ok(new { success = false, messegeEror = "Статьи не найдены" });

                return Ok(new { success = true, list });

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, messegeEror = $"Ошибка: {ex.Message}" });
            }

        }

        [HttpGet]
        [Route("GetCountArticle")]
        public async Task<IActionResult> GetCountArticle()
        {
            int countPost = 0;
            try
            {
                var email = User?.FindFirst(ClaimTypes.Email)?.Value;
                countPost = await _articleService.CountArticleWroteByUserAsync(email);
                return Ok(countPost);
            }
            catch
            {
                return Ok(countPost);
            }
        }
    }
}

