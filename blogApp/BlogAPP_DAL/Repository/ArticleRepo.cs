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
            if (entry.State == EntityState.Detached)
                await _context.Articles.AddAsync(article);

            await _context.SaveChangesAsync();
        }

        public async Task<Article> GetArticleByIdAsync(string id)
        {
            return await _context.Articles.FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<List<string>> GetTagNamesByArticleIdAsync(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
                return new List<string>();

            return await _context.Article_Tags
                .Where(x => x.Article_id == articleId)
                .Select(x => x.Tag.Name)
                .ToListAsync();
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
            return await _context.Articles.Where(x => x.Title == propertiesFind.Title).ToListAsync();
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

        public async Task<List<Article>> GetArticleByTitileANDTagsAsync(string title, IEnumerable<string> tags)
        {
            if (string.IsNullOrEmpty(title) || tags == null || !tags.Any())
                return new List<Article>();
            var normalized = tags.Select(t => t.Trim().ToLower()).ToList();
            var query = _context.Articles
                .Where(a => a.Title == title)
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


        public async Task<bool> DeleteArticleByIdAsync(string articleId)
        {
            var article = await _context.Articles.FirstOrDefaultAsync(x => x.Id == articleId);
            if (article == null)
                return false;

            var articleTags = await _context.Article_Tags.Where(x => x.Article_id == articleId).ToListAsync();
            if (articleTags.Any())
                _context.Article_Tags.RemoveRange(articleTags);

            var comments = await _context.Comments.Where(x => x.ArticleId == articleId).ToListAsync();
            if (comments.Any())
                _context.Comments.RemoveRange(comments);

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateArticleAsync(Article article)
        {
            var existing = await _context.Articles.FirstOrDefaultAsync(x => x.Id == article.Id);
            if (existing == null)
                return false;

            existing.Title = article.Title;
            existing.Text = article.Text;
            existing.Description = article.Description;
            existing.Cover_image = article.Cover_image;
            existing.ReadTime = article.ReadTime;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
