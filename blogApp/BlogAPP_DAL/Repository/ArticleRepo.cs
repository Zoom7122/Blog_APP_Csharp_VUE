using BlogAPP_Core.Models;
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

        public async Task CreateArticleinDbAsync(Article article)
        {
            var entry = _context.Entry(article);
            if(entry.State == EntityState.Detached)
                await _context.Articles.AddAsync(article);

            await _context.SaveChangesAsync();
        }

        public async Task<Article> GetArticleByIdAsync(string id)
        {
            return await _context.Articles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            return await _context.Articles.ToListAsync();
        }

        public async Task<List<Article>> GetArticlesByEmailAthorAsync(string emailAthor)
        {
            return await _context.Articles.Where(x => x.Author_Email == emailAthor).ToListAsync();
        }

        public async Task<int> GetCountArticleInDbPostByUserAsync(string email)
        {
            return await _context.Articles.CountAsync(x => x.Author_Email == email);
        }

        public async Task<List<Article>> GetArticleByTitileAsync(string titile)
        {
            return await _context.Articles.Where(x => x.Title == titile).ToListAsync();
        }

        public async Task<List<Article>> GetArticleByTitileANDTagAsync(
            ArticlePropertiesFind propertiesFind)
        {
            return await _context.Articles.Where(x =>x.Title == propertiesFind.Title).ToListAsync();
        }

        public async Task<List<Article>> GetArticlesByTagsAsync(IEnumerable<string> tags)
        {
            if (tags == null || !tags.Any())
                return new List<Article>();

            var normalized = tags.Select(t => t.Trim().ToLower()).ToList();

            var query = _context.Articles
                .Where(a => _context.Article_Tags
                    .Where(at => normalized.Contains(at.Tag.Name.ToLower()))
                    .Select(at => at.Article_id)
                    .Contains(a.Id));


            query = query.Where(a => normalized.All(t => _context.Article_Tags
                            .Where(at => at.Article_id == a.Id)
                            .Select(at => at.Tag.Name.ToLower())
                            .Contains(t)));
            

            return await query.ToListAsync();
        }
    }
}
