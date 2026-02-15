using AutoMapper;
using BlogAPP_BLL.Exceptions;
using BlogAPP_BLL.Intarface;
using BlogAPP_BLL.Models;
using BlogAPP_Core.Models;
using blogApp_DAL.Intarface;
using blogApp_DAL.Model;
using BlogAPP_DAL.Intarface;
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

            List<string> tags = model.Tag; 

            for(int i=0;i <tags.Count; i++)
            {
                await _tagService.CreatrTagToArticleAsync(tags[i], article.Id);
            }

            return true;
        }


        public async Task<List<ArticleReturnInAPI>> FindArticleByProperties(ArticlePropertiesFind properties)
        {
            if (properties == null)
                return null;

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

            }
            return listArticleToPush;
        }

        private async Task<List<Article>> GetArticlesByCriteria(ArticlePropertiesFind properties)
        {

            if (properties == null)
                return null;

            if (properties.Tags != null && properties.Tags.Count > 0)
            {
                return await _articleRepo.GetArticlesByTagsAsync(properties.Tags);
            }

            return properties.Title switch
            {
                null => null,
                string title => await _articleRepo.GetArticleByTitileAsync(title),
            };
        }
    }
}
