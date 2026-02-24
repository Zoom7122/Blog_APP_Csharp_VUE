using AutoMapper;
using BlogAPP_BLL.Exceptions;
using BlogAPP_BLL.Intarface;
using BlogAPP_BLL.Models;
using BlogAPP_Core.Models;
using blogApp_DAL.Intarface;
using blogApp_DAL.Model;
using BlogAPP_DAL.Intarface;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogAPP_BLL.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IMapper _mapper;
        private readonly IArticleRepo _articleRepo;
        private readonly ICommentsService _commentsService;
        private readonly ITagService _tagService;

        public ArticleService(IArticleRepo articleRepo, IMapper mapper,
            ICommentsService commentsService, ITagService tagService)
        {
            _articleRepo = articleRepo;
            _mapper = mapper;
            _commentsService = commentsService;
            _tagService = tagService;
        }

        public async Task<int> CountArticleWroteByUserAsync(string email)
        {
            return await _articleRepo.GetCountArticleInDbPostByUserAsync(email);
        }


        public async Task<bool> CreateArticle(CreateArticleModel model, UserCookie userCooki)
        {
            if (model.Title == null)
                throw new ArticleException("Заполните название");

            if (model.Text == null)
                throw new ArticleException("Текст не null");

            if (model.ReadTime <= 0)
                throw new ArticleException("Заполните время чтения");

            if (model.Description == null)
                throw new ArticleException("Заполните описание");

            if (userCooki == null || userCooki.Email == null)
                throw new ArticleException("Необходим email");

            var article = _mapper.Map<Article>(model);

            article.Author_Email = userCooki.Email;

            article.PublishedAt = DateTime.Now;

            await _articleRepo.CreateArticleinDbAsync(article);


            //далее добавление тегов
            if (model.Tag.Count > 0)
            {
                List<string> tags = new List<string>();

                for (int i = 0; i < model.Tag.Count;i ++)
                {
                    if (!string.IsNullOrEmpty(model.Tag[i])) tags.Add(model.Tag[i]);
                }
                if (tags.Count > 0)
                {
                    for (int i = 0; i < tags.Count; i++)
                    {
                        await _tagService.CreatrTagToArticleAsync(tags[i], article.Id);
                    }
                }
            }

            return true;
        }

        public async Task<bool> DeleteArticleAsync(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
                throw new ArticleException("Не указан ID статьи");

            return await _articleRepo.DeleteArticleByIdAsync(articleId);
        }

        public async Task<ArticleReturnInAPI> FindArticleByID(string articleId)
        {
            var article = await _articleRepo.GetArticleByIdAsync(articleId);
            if (article == null)
                return null;

            var articleToPush = _mapper.Map<ArticleReturnInAPI>(article);
            articleToPush.Tags = await _articleRepo.GetTagNamesByArticleIdAsync(articleId) ?? new List<string>();

            return articleToPush;
        }

        public async Task<bool> UpdateArticleAsync(UpdateArticleModel model, string userEmail)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Id))
                throw new ArticleException("Некорректные данные статьи");

            var article = await _articleRepo.GetArticleByIdAsync(model.Id);
            if (article == null)
                throw new ArticleException("Статья не найдена");

            if (article.Author_Email != userEmail)
                throw new ArticleException("Недостаточно прав для редактирования статьи");

            article.Title = model.Title;
            article.Text = model.Text;
            article.Description = model.Description;
            article.Cover_image = model.CoverImage;
            article.ReadTime = model.ReadTime;

            var result = await _articleRepo.UpdateArticleAsync(article);
            if (!result)
                return false;

            await _tagService.SyncArticleTagsAsync(article.Id, model.Tag);
            return true;
        }

        public async Task<List<ArticleReturnInAPI>> FindArticleByProperties(ArticlePropertiesFind properties)
        {
            if (properties == null)
                return null;

            if (string.IsNullOrEmpty(properties.Title) && (properties.Tags == null || properties.Tags.Count <= 0)) return null;

            var listArticle = await GetArticlesByCriteria(properties);

            var listArticleToPush = listArticle?.Select(article => _mapper.Map<ArticleReturnInAPI>(article)).ToList();

            for (int i =0; i< listArticle.Count;i++)
            {

                var comments = await _commentsService.ArticleComments(listArticle[i]);
                if (comments != null)
                {
                    for (int j = 0; j < comments.Count; j++)
                    {
                        listArticleToPush[i].comments.Add(comments[j]);
                    }
                }
                var tags = await _articleRepo.GetTagNamesByArticleIdAsync(listArticle[i].Id);
                listArticleToPush[i].Tags = tags ?? new List<string>();

            }
            return listArticleToPush;
        }

        public async Task<List<ArticleReturnInAPI>> FindArticleWroteByuser(string emailUser)
        {
             var listArticle = await _articleRepo.GetArticlesByEmailAthorAsync(emailUser);

            var listToPush = new List<ArticleReturnInAPI>();

            for (int i = 0; i < listArticle.Count; i++)
            {
                listToPush.Add(_mapper.Map<ArticleReturnInAPI>(listArticle[i]));
            }
            
            return listToPush;
        }

        private async Task<List<Article>> GetArticlesByCriteria(ArticlePropertiesFind properties)
        {

            if (properties == null)
                return null;

            if (properties.Tags == null || properties.Tags.Count <= 0) properties.Tags = null;

            if (string.IsNullOrEmpty(properties.Title)) properties.Title = null;


            return (properties.Title, properties.Tags) switch
            {
                (null, null) => null,
                (string title, null) => await _articleRepo.GetArticleByTitileAsync(title),
                (null, List<string> tags) => await _articleRepo.GetArticlesByTagsAsync(tags),
                (string title, IEnumerable<string> tags) => await _articleRepo.GetArticleByTitileANDTagsAsync(title, tags)
            };

        }
    }
}
