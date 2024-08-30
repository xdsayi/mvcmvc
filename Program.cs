using mvcmvc.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login"; // Kullan�c� giri� yapmad���nda y�nlendirilecek URL
        options.AccessDeniedPath = "/Home/Index"; // Kullan�c� yetkisiz eri�im denedi�inde y�nlendirilecek URL
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

//database i�in


builder.Services.AddDbContext<BlogContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BlogContext")));
// blog database i�in


builder.Services.AddDbContext<FilmContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("FilmContext")));// film database i�in
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
