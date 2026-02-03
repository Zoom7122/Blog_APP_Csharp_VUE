using BlogAPP_BLL.Intarface;
using BlogAPP_BLL.Mappings;
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


var dbPath = GetPath.GetDatabasePath();
Console.WriteLine($"Путь к БД: {dbPath}");
Console.WriteLine($"Файл существует: {File.Exists(dbPath)}");
Console.WriteLine($"Полный путь: {Path.GetFullPath(dbPath)}");

var connectionString = $"Data source={GetPath.GetDatabasePath()}";
builder.Services.AddDbContext<Blog_DBcontext>(opt => opt.UseSqlite(connectionString));

builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IArticleRepo, ArticleRepo>();

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

// 3. Добавляем Swagger для тестирования API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Конфигурация middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseCors("AllowVue");
app.UseAuthorization();
app.MapControllers();

app.Run();