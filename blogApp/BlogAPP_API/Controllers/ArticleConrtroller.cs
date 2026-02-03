using BlogAPP_BLL.Intarface;
using BlogAPP_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime;

namespace BlogAPP_API.Controllers
{
    //[Authorize]
    [AllowAnonymous]
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
                var result = await _articleService.CreateArticle(model);

                if (result)
                {
                    return Ok(new { success = true });
                }
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
