using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using NetworkInfrastructure.Web.Data.Context;
using NetworkInfrastructure.Web.Data.Services;
using NetworkInfrastructure.Web.Models;
using NetworkInfrastructure.Web.Models.Profile;
using NetworkInfrastructure.Web.Models.Validate;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<NetworkContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("NetConnection"));
});

builder.Services.AddAutoMapper(typeof(Profiler));

builder.Services.AddScoped<INetworkService, NetworkService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IValidator<UserDto>, UserDtoValidation>();

builder.Services.AddScoped<IValidator<NetworkAssetDto>, NetworkAssetDtoValidation>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x =>
{
    x.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    x.SlidingExpiration = true;
    x.LoginPath = new PathString("/Network/Login/");
    x.AccessDeniedPath = new PathString("/Network/Forbidden/");
    x.Cookie.Name = "my_delicious_little_cookies";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.Strict
});

app.UseRouting();


app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Network}/{action=Login}/{id?}");

app.Run();
