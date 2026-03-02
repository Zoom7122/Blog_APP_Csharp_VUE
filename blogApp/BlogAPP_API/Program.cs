using BlogAPP_BLL.DependencyInjection;
using BlogAPP_BLL.Intarface;
using BlogAPP_BLL.Mappings;
using BlogAPP_BLL.Models;
using BlogAPP_BLL.Services;
using BlogAPP_Core;
using blogApp_DAL;
using blogApp_DAL.Intarface;
using blogApp_DAL.Repository;
using BlogAPP_DAL;
using BlogAPP_DAL.Intarface;
using BlogAPP_DAL.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.Name = "AuthCookie";
        options.LoginPath = "/api/Entrance/Login";
        options.LogoutPath = "/api/Entrance/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.SlidingExpiration = true;
    });

var cs = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(cs))
{
    cs = $"Data source={GetPath.GetDatabasePath()}";
}
builder.Services.AddDbContext<Blog_DBcontext>(opt => opt.UseSqlite(cs));


builder.Services.AddDal();
builder.Services.AddBll();

builder.Services.AddTransient<BlogAPP_API.Middleware.ExceptionHandlingMiddleware>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var allowedOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue", policy =>
    {
        policy.WithOrigins(allowedOrigins)  // адрес Vue
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


builder.Services.AddAuthorization();

//свагер
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

app.UseMiddleware<BlogAPP_API.Middleware.ExceptionHandlingMiddleware>();

//свагер development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("AllowVue");     

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();