
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BookingApp.Data;
using BookingApp.Models;
using System.Configuration;
using DNTCaptcha.Core;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddDNTCaptcha(option =>
{
    option.UseCookieStorageProvider().ShowThousandsSeparators(false);
    option.WithEncryptionKey("abdgTJHGYIOUTGH12fhsj87668KJH");
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}


app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;

    headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
    headers["X-Content-Type-Options"] = "nosniff";
    headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    headers["Permissions-Policy"] = "geolocation=(), camera=(), microphone=()";
    headers["Cross-Origin-Opener-Policy"] = "same-origin";
    headers["Cross-Origin-Resource-Policy"] = "same-origin";
    
    headers.Remove("X-Powered-By");
    headers.Remove("Server");
    headers.Remove("X-ASPNET-VERSION");
    headers.Remove("X-POWERED-BY-PLESK");


    await next.Invoke();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "Home",
    pattern: "{controller=Home}/{action=Default}/{id?}");

app.MapRazorPages();

app.Run();
