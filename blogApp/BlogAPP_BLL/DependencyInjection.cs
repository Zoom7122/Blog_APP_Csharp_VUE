using BlogAPP_BLL.Intarface;
using BlogAPP_BLL.Mappings;
using BlogAPP_BLL.Models;
using BlogAPP_BLL.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogAPP_BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBll(this IServiceCollection services)
        {
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<ICommentsService, CommentsService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddAutoMapper(typeof(AutoMapperProfile));
            return services;
        }
    }
}
