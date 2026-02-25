using BlagAPP_MVC.Models;
using BlogAPP_BLL.Intarface;
using BlogAPP_BLL.Models;
using BlogAPP_BLL.Services;
using BlogAPP_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlagAPP_MVC.Controllers
{
    [Authorize]
    //[AllowAnonymous]
    public class ArticleController : Controller
    {

        private readonly IArticleService _articleService;
        private readonly ICommentsService _commentsService;


        public ArticleController(
        IArticleService articleService,
        ICommentsService commentsService)
        {
            _articleService = articleService;
            _commentsService = commentsService;
        }

        [HttpGet]
        [Route("CreateArticle")]
        public async Task<IActionResult> CreateArticle()
        {
            return View(new CreateArticleModel() { ReadTime = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreateArticle")]
        public async Task<IActionResult> CreateArticle(
            CreateArticleModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var allClimes = User.Claims.ToList();

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
                {
                    TempData["SuccessMessage"] = "Статья успешно добавлена!";
                    return RedirectToAction(nameof(CreateArticle));
                }
                ModelState.AddModelError("", "Не удалось сохранить статью.");
                return View(model); 

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Произошла ошибка при сохранении статьи.");
                return View(model);
            }
        }


        [HttpGet]
        [Route("ArticleView")]
        public IActionResult ArticleView()
        {
            return View(new ArticleSearchViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ArticleView")]
        public async Task<IActionResult> ArticleView(ArticleSearchViewModel model)
        {
            var properties = new ArticlePropertiesFind
            {
                Title = string.IsNullOrWhiteSpace(model.Title) ? null : model.Title.Trim(),
                Tags = string.IsNullOrWhiteSpace(model.Tag) ? null : new List<string> { model.Tag.Trim() }
            };

            if (model.Title == null && model.Tag == null)
            {
                var articles = await _articleService.GetAllArticles();
                model.Articles = articles ?? new List<ArticleReturnInAPI>();
                model.SearchCompleted = true;
            }
            else
            {
                var articles = await _articleService.FindArticleByProperties(properties);
                model.Articles = articles ?? new List<ArticleReturnInAPI>();
                model.SearchCompleted = true;
            }

            return View(model);
        }

        [HttpGet]
        [Route("MyArticle")]
        public async Task<IActionResult> MyArticle()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            var listArticle = await _articleService.FindArticleWroteByuser(userEmail);

            if (listArticle == null) TempData["ListArticleIsNull"] = "У вас пока нет статей";

            return View(listArticle);

        }

        [HttpGet]
        [Route("UpdateArticle/{articleId}")]
        public async Task<IActionResult> UpdateArticle(string articleId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var article = await _articleService.FindArticleByID(articleId);

            if (article == null)
            {
                TempData["Error"] = "Статья не найдена";
                return RedirectToAction(nameof(MyArticle));
            }

            if (userEmail != article.Author_Email)
            {
                TempData["Error"] = "Недостаточно прав для редактирования статьи";
                return RedirectToAction(nameof(MyArticle));
            }

            var model = new UpdateArticleModel
            {
                Id = article.Id,
                Title = article.Title,
                Description = article.Description,
                Text = article.Text,
                CoverImage = article.Cover_image,
                ReadTime = article.ReadTime ?? 1,
                Tag = article.Tags ?? new List<string>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UpdateArticle")]
        public async Task<IActionResult> UpdateArticlePost(UpdateArticleModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("UpdateArticle", model);
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            try
            {
                var result = await _articleService.UpdateArticleAsync(model, userEmail);
                if (result)
                {
                    TempData["SuccessMessage"] = "Статья успешно обновлена";
                    return RedirectToAction(nameof(MyArticle));
                }

                ModelState.AddModelError("", "Не удалось обновить статью");
                return View("UpdateArticle", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("UpdateArticle", model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("DeleteArticle/{articleId}")]
        public async Task<IActionResult> DeleteArticle(string articleId)
        {
            try
            {
                var result = await _articleService.DeleteArticleAsync(articleId);

                if (!result)
                {
                    TempData["Error"] = "Статья не найдена";
                }
                else
                {
                    TempData["SuccessMessage"] = "Статья успешно удалена";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка при удалении статьи: {ex.Message}";
            }

            return RedirectToAction(nameof(MyArticle));
        }
    }
}

