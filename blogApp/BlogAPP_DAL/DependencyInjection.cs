using blogApp_DAL.Intarface;
using blogApp_DAL.Repository;
using BlogAPP_DAL.Intarface;
using BlogAPP_DAL.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogAPP_DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDal(this IServiceCollection services)
        {
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IArticleRepo, ArticleRepo>();
            services.AddScoped<ICommentsRepo, CommentsRepo>();
            services.AddScoped<ITagRepo, TagRepo>();
            services.AddScoped<IArticle_TagRepo, Article_TagRepo>();
            return services;
        }
    }
}
