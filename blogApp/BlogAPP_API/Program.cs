using BlogAPP_BLL.Intarface;
using BlogAPP_BLL.Mappings;
using BlogAPP_BLL.Models;
using BlogAPP_BLL.Services;
using BlogAPP_Core;
using blogApp_DAL;
using blogApp_DAL.Intarface;
using blogApp_DAL.Repository;
using BlogAPP_DAL.Intarface;
using BlogAPP_DAL.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.Name = "AuthCookie";
        options.LoginPath = "/api/EntranceConroller/Login";
        options.LogoutPath = "/api/EntranceConroller/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.SlidingExpiration = true;
    });

var cs = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(cs))
{
    cs = $"Data source={GetPath.GetDatabasePath()}";
}
builder.Services.AddDbContext<Blog_DBcontext>(opt => opt.UseSqlite(cs));


builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IArticleRepo, ArticleRepo>();
builder.Services.AddScoped<ICommentsRepo, CommentsRepo>();
builder.Services.AddScoped<ICommentsService, CommentsService>();
builder.Services.AddScoped<ITagRepo, TagRepo>();
builder.Services.AddScoped<IArticle_TagRepo, Article_TagRepo>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();



builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue", policy =>
    {
        policy.WithOrigins("http://localhost:5173")  // адрес Vue
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


//свагер
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


    var app = builder.Build();

//свагер development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { message = "Ошибка сервера" });
    });
});

app.UseHttpsRedirection();

app.UseCors("AllowVue");     

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();