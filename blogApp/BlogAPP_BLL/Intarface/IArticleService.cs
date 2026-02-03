using BlogAPP_Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogAPP_BLL.Intarface
{
    public interface IArticleService
    {
        Task<bool> CreateArticle( CreateArticleModel model);

    }
}
