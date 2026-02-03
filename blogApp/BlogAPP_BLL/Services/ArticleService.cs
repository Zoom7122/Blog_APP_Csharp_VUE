using AutoMapper;
using BlogAPP_BLL.Exceptions;
using BlogAPP_BLL.Intarface;
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

        public ArticleService(IArticleRepo articleRepo, IMapper mapper)
        {
            _articleRepo = articleRepo;
            _mapper = mapper;
        }

        public async Task<bool> CreateArticle(CreateArticleModel model)
        {
            if (model.Title == null)
                throw new ArticleException("Заполните название");

            if (model.text == null)
                throw new ArticleException("Текст не null");

            if (model.ReadTime <= 0)
                throw new ArticleException("Заполните время чтения");

            if (model.description == null)
                throw new ArticleException("Заполните описание");

            var article = _mapper.Map<Article>(model);

            _articleRepo.CreateArticleinDbAsync(article);

            return true;
        }

    }
}
