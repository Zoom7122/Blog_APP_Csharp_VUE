using BlogAPP_BLL.Intarface;
using blogApp_DAL;
using blogApp_DAL.Model;
using BlogAPP_DAL.Intarface;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BlogAPP_BLL.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepo _tagRepo;
        private readonly IArticle_TagRepo _article_TagRepo;

        public TagService(ITagRepo tagRepo, IArticle_TagRepo article_TagRepo)
        {
            _tagRepo = tagRepo;
            _article_TagRepo = article_TagRepo;
        }

        public async Task CreatrTagToArticleAsync(string tag, string articleId)
        {
            var existTagInDb = await _tagRepo.FindTagByName(tag);

            if (existTagInDb != null)
            {
                var article_TagCreateConnection = new Article_Tag()
                {
                    Tag_id = existTagInDb.Id,
                    Article_id = articleId,
                    Created_at = DateTime.Now,
                };

                await _article_TagRepo.CreateRowTable(article_TagCreateConnection);
                return;
            }

            var tagToPush = new Tag()
            { 
                Name = tag,
                Created_At = DateTime.Now,
                Id = Guid.NewGuid().ToString()
            };

            await _tagRepo.CreateTagAsync(tagToPush);

            var articler_Tag = new Article_Tag()
            {
                Tag_id = tagToPush.Id,
                Article_id = articleId,
                Created_at = DateTime.Now,
            };

            await _article_TagRepo.CreateRowTable(articler_Tag);

            return;
        }

        public async Task SyncArticleTagsAsync(string articleId, List<string> tags)
        {
            await _article_TagRepo.DeleteRowsByArticleIdAsync(articleId);

            if (tags == null || tags.Count == 0)
                return;

            var normalizedTags = tags
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            for (int i = 0; i < normalizedTags.Count; i++)
            {
                await CreatrTagToArticleAsync(normalizedTags[i], articleId);
            }
        }

    }
}
