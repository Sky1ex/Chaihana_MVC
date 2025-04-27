using Mapster;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers;
using WebApplication1.DataBase;
using WebApplication1.DataBase_and_more;
using WebApplication1.DTO;
using WebApplication1.OtherClasses;
using WebApplication1.Repository.Default;
using WebApplication1.Services;
using WebApplication1.Exceptions;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/*builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");*/

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidAudience = AuthOptions.AUDIENCE,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey()
        };
        // ������ ������ �� Cookie
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["jwt_token"];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Authorization"); // ����� ��� JWT
    });
});

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ��������� ������
var config = new TypeAdapterConfig();
new MappingConfig().Register(config);

builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, Mapper>();

builder.Services.AddHttpContextAccessor(); // ��������� ��������� IHttpContextAccessor
builder.Services.AddScoped<UserService>(); // ������������ UserService
builder.Services.AddScoped<AccountService>(); // ������������ UserService
builder.Services.AddScoped<MenuService>(); // ������������ MenuService
builder.Services.AddScoped<BookingService>(); // ������������ BookingService

builder.Services.AddEndpointsApiExplorer(); //
builder.Services.AddSwaggerGen(); // ���������� swagger(��� ������ � ��)

builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddExceptionHandler<ExceptionHandler>(); // ��������� ���������� ������ ��� �������

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    /*app.UseExceptionHandler("/Home/Error");*/
    app.UseExceptionHandler(_ => { });
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
}

app.UseExceptionHandler(errorApp =>
{
	errorApp.Run(async context =>
	{
		var error = context.Features.Get<IExceptionHandlerFeature>();
		if (error == null) return;

		var exception = error.Error;
		var isApiRequest = context.Request.Path.StartsWithSegments("/api");

		var model = new ErrorViewModel
		{
			Message = ErrorViewModel.GetUserFriendlyMessage(exception),
			Details = context.Response.StatusCode == 500 ? exception.Message : null,
			ValidationErrors = (exception as ValidationException)?.Errors
		};

		if (isApiRequest)
		{
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsJsonAsync(model);
		}
		else
		{
			context.Response.ContentType = "text/html";
			var result = new ViewResult
			{
				ViewName = "Error",
				ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
				{
					Model = model
				}
			};
			await result.ExecuteResultAsync(new ActionContext
			{
				HttpContext = context,
				RouteData = new RouteData(),
				ActionDescriptor = new ActionDescriptor()
			});
		}
	});
});

app.UseSwagger(); // 
app.UseSwaggerUI(); // ������ ��� swagger

app.UseHttpsRedirection();
app.UseStaticFiles(); // ��� ������ � ��������� ����

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

app.UseMiddleware<AutoLoginMiddleware>();

app.Run();
