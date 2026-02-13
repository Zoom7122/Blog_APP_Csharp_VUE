using BlogAPP_BLL.Intarface;
using BlogAPP_BLL.Models;
using BlogAPP_BLL.Services;
using BlogAPP_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogAPP_API.Controllers
{

    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;
        private readonly ILoginService _loginService;

        public CommentsController(ICommentsService commentsService, ILoginService loginService)
        {
            _commentsService = commentsService;
            _loginService = loginService;
        }

        [HttpPost]
        [Route("CreateComments")]
        public async Task<IActionResult> CreateComments(
        [FromBody] CommentModelsCreate comment)
        {
            try
            {
                var commetsToPush = comment;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var user = await _loginService.FindUserByEmail(email);

                commetsToPush.UserId = user.Id;


                var result = await _commentsService.CreateComments(commetsToPush);
                if(result)
                    return Ok(new { success = true });

                else return Ok(new { success = false});
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, messegeEror = $"Ошибка: {ex.Message}" });
            }

        }

        [HttpGet]
        [Route("GetCountComments")]
        public async Task<IActionResult> GetCountComments()
        {
            try
            {
                int countCommets = 0;

                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var user = await _loginService.FindUserByEmail(userEmail);

                countCommets = await _commentsService.GetCountCommentsWroteByUser(user.Id);
                if (countCommets == null)  return Ok(new { countCommets = 0, errorMessege = "У вас нет комментариев" });
                if (countCommets > 0)
                    return Ok(new { countCommets });
                else return Ok(new { countCommets , errorMessege ="У вас нет комментариев"});

            }
            catch (Exception ex)
            {
                return Ok(new { countCommets = 0 ,errorMessege = $"Произошла ошибка: {ex.Message}" });
            }
        }
    }
}
