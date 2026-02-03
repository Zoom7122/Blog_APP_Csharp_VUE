using blogApp_DAL;
using blogApp_DAL.Model;
using BlogAPP_DAL.Intarface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlogAPP_DAL.Repository
{
    public class ArticleRepo : IArticleRepo
    {
        private readonly Blog_DBcontext _context;

        public ArticleRepo(Blog_DBcontext context)
        {
            _context = context;
        }

        public async void CreateArticleinDbAsync(Article article)
        {
            var entry = _context.Entry(article);
            if(entry.State == EntityState.Detached)
                await _context.Articles.AddAsync(article);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Article>> GetAllArticleByNameAuthor(string name)
        {
            return await _context.Articles.Where(
                x => x.Author_Name == name).ToListAsync();
        }

        public async Task<Article> GetArticleByIdAsync(string id)
        {
            return await _context.Articles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Article>> GetArticleByTagAsync(string tag)
        {
            return await _context.Articles.Where(x => x.Tag == tag).ToListAsync();
        }

        public async Task<List<Article>> GetArticlesAsync()
        {
            return await _context.Articles.ToListAsync();
        }

        public int GetCountArticleInDbPostByUser(string name)
        {
            return _context.Articles.Count();
        }
    }
}
