using BlogAPP_BLL.DependencyInjection;
using BlogAPP_Core;
using blogApp_DAL;
using BlogAPP_DAL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var logger = LogManager.Setup()
.LoadConfigurationFromFile("nlog.config")
.GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();


    builder.Services.AddControllersWithViews();


    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.Name = "MvcAuthCookie";
            options.LoginPath = "/Login";
            options.LogoutPath = "/Logout";
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

    builder.Services.AddAuthorization();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {

        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseExceptionHandler("/Error");

    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }


    app.UseHttpsRedirection();
    app.UseRouting();


    app.UseAuthentication();
    app.UseAuthorization();

    app.UseStatusCodePagesWithReExecute("/Error/StatusCode/{0}");

    app.MapGet("/", () => Results.Redirect("/Login"));

    app.MapStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}")
        .WithStaticAssets();

    app.Run();
}
catch(Exception ex)
{
    logger.Error(ex, "Application stopped because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
