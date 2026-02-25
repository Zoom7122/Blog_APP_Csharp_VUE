using BlagAPP_MVC.Models;
using BlogAPP_BLL.Intarface;
using BlogAPP_BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlagAPP_MVC.Controllers
{

    [Authorize]
    public class CommentsController : Controller
    {

        private readonly ICommentsService _commentsService;
        private readonly ILoginService _loginService;

        public CommentsController(ICommentsService commentsService, ILoginService loginService)
        {
            _commentsService = commentsService;
            _loginService = loginService;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Comments/Add")]
        public async Task<IActionResult> Add(AddCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["CommentError"] = "Проверьте корректность заполнения комментария.";
                return RedirectToAction("ArticleView", "Article", new { title = model.SearchTitle, tag = model.SearchTag });
            }

            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrWhiteSpace(userEmail))
                {
                    TempData["CommentError"] = "Не удалось определить текущего пользователя.";
                    return RedirectToAction("ArticleView", "Article", new { title = model.SearchTitle, tag = model.SearchTag });
                }

                var user = await _loginService.FindUserByEmail(userEmail);

                await _commentsService.CreateComments(new CommentModelsCreate
                {
                    ArticleId = model.ArticleId,
                    Content = model.Content,
                    UserId = user.Id
                });

                TempData["CommentSuccess"] = "Комментарий успешно добавлен.";
            }
            catch (Exception ex)
            {
                TempData["CommentError"] = $"Ошибка при добавлении комментария: {ex.Message}";
            }

            return RedirectToAction("ArticleView", "Article", new { title = model.SearchTitle, tag = model.SearchTag });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("Comments/Delete/{commentId}")]
        public async Task<IActionResult> Delete(string commentId, string? searchTitle, string? searchTag)
        {
            try
            {
                var result = await _commentsService.DeleteCommentAsync(commentId);

                if (!result)
                {
                    TempData["CommentError"] = "Комментарий не найден.";
                }
                else
                {
                    TempData["CommentSuccess"] = "Комментарий удалён.";
                }
            }
            catch (Exception ex)
            {
                TempData["CommentError"] = $"Ошибка при удалении комментария: {ex.Message}";
            }

            return RedirectToAction("ArticleView", "Article", new { title = searchTitle, tag = searchTag });
        }
    }
}
