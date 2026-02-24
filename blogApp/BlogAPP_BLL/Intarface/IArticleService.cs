using BlogAPP_BLL.Models;
using BlogAPP_BLL.Services;
using BlogAPP_Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogAPP_BLL.Intarface
{
    public interface IArticleService
    {
        Task<bool> CreateArticle( CreateArticleModel model, UserCookie userCooki);

        Task<int> CountArticleWroteByUserAsync(string email);

        Task<List<ArticleReturnInAPI>> FindArticleByProperties(ArticlePropertiesFind properties);

        Task<bool> DeleteArticleAsync(string articleId);

        Task<List<ArticleReturnInAPI>> FindArticleWroteByuser(string emailUser);

        Task<ArticleReturnInAPI> FindArticleByID(string articleId);

        Task<bool> UpdateArticleAsync(UpdateArticleModel model, string userEmail);

    }
}
