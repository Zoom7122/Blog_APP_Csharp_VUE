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

            var articles = await _articleService.FindArticleByProperties(properties);
            model.Articles = articles ?? new List<ArticleReturnInAPI>();
            model.SearchCompleted = true;

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

        [HttpPost]
        [Route("UpdateArticle")]
        public async Task<IActionResult> UpdateArticle(string ArticleID)
        {
            var userEmail =User.FindFirst(ClaimTypes.Email)?.Value;

            var article = await _articleService.FindArticleByID(ArticleID);

            if (userEmail != article?.Author_Email)
                TempData["Error"] = "Ошибка сопоставления email";

            return View(article);
        }

    }
}
