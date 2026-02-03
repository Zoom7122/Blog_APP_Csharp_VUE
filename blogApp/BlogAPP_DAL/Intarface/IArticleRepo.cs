using blogApp_DAL.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogAPP_DAL.Intarface
{
    public interface IArticleRepo
    {
        Task<List<Article>> GetArticlesAsync();

        Task<Article> GetArticleByIdAsync(string id);

        Task<List<Article>> GetArticleByTagAsync(string tag);

        Task<List<Article>> GetAllArticleByNameAuthor(string name);

        void CreateArticleinDbAsync(Article article);

        int GetCountArticleInDbPostByUser(string name);
    }
}
