using blogApp_DAL.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogAPP_DAL.Intarface
{
    public interface IArticle_TagRepo
    {
        Task CreateRowTable(Article_Tag article_Tag);

        Task DeleteRowsByArticleIdAsync(string articleId);
    }
}
