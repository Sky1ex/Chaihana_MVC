using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers;
using WebApplication1.DataBase;
using WebApplication1.OtherClasses;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor(); // Добавляем поддержку IHttpContextAccessor
builder.Services.AddScoped<UserService>(); // Регистрируем UserService

builder.Services.AddEndpointsApiExplorer(); //
builder.Services.AddSwaggerGen(); // добавление swagger(для работы с бд)

builder.Services.AddScoped<ICartService, CartService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
}

app.UseSwagger(); // 
app.UseSwaggerUI(); // методы для swagger

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "products",
    pattern: "Menu/{action=Index}/{id?}",
    defaults: new { controller = "Menu" });

/*app.MapControllerRoute(
    name: "addtocart",
    pattern: "Cart/AddToCart/{action=AddToCart}/{id?}",
    defaults: new { controller = "Cart" });*/

/*app.MapControllerRoute(
    name: "showcart",
    pattern: "Cart/ShowCart/{action=ShowCart}/{id?}");*/

/*app.MapControllerRoute(
    name: "purshare",
    pattern: "Cart/Purshare/{action=Purshare}/{id?}");*/

app.MapControllerRoute(
    name: "account",
    pattern: "Account/{action=Index}/{id?}",
    defaults: new { controller = "Account" });

app.MapControllerRoute(
    name: "adresses",
    pattern: "Account/Adresses/{action=Adresses}/{id?}");

app.MapControllerRoute(
    name: "orders",
    pattern: "Account/Orders/{action=Orders}/{id?}");

app.UseMiddleware<AutoLoginMiddleware>();

app.Run();
