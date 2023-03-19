using ChannelServices.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopper.Models;
using ShopWeb.Data;
using ShopWeb.Models.Sevices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ISendEmail, SendEmail>();
builder.Services.AddScoped<IGetProducts, GetProducts>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.";

    options.Lockout.MaxFailedAccessAttempts = 6;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(7);
})
    .AddEntityFrameworkStores<ShopWebContext>()
    .AddDefaultTokenProviders().AddErrorDescriber<ErrorString>();

builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "sdfuibfuiahkhsdfushdukfhsnksdfuibs-0jksdsdf";
    options.ClientSecret = "123dc2f2342d389dfh23r7h2v";
});

#region Use SqlServer
builder.Services.AddDbContext<ShopWebContext>(options =>
{
    options.UseSqlServer("Data Source =.; Initial Catalog = ShopWeb_Db; Integrated Security = true");
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
