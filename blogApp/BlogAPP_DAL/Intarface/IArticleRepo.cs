using BlogAPP_Core.Models;
using blogApp_DAL.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogAPP_DAL.Intarface
{
    public interface IArticleRepo
    {
        Task<List<Article>> GetAllArticlesAsync();

        Task<Article> GetArticleByIdAsync(string id);

        Task CreateArticleinDbAsync(Article article);

        Task<List<Article>> GetArticlesByEmailAthorAsync(string emailAthor);

        Task<int> GetCountArticleInDbPostByUserAsync(string email);

        Task<List<Article>> GetArticleByTitileANDTagAsync(ArticlePropertiesFind propertiesFind);

        Task<List<Article>> GetArticlesByTagsAsync(IEnumerable<string> tags);

        Task<List<Article>> GetArticleByTitileAsync(string titile);
        Task<List<Article>> GetArticleByTitileANDTagsAsync(string title, IEnumerable<string> tags);

        Task<List<string>> GetTagNamesByArticleIdAsync(string articleId);
        Task<bool> DeleteArticleByIdAsync(string articleId);

        Task<bool> UpdateArticleAsync(Article article);

    }
}
