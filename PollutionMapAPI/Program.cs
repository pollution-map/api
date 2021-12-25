using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PollutionMapAPI.DataAccess;
using PollutionMapAPI.Middleware;
using PollutionMapAPI.Models;
using PollutionMapAPI.Repositories.Core;
using PollutionMapAPI.Services.Auth;
using PollutionMapAPI.Services.Email;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure app to run on specific port if specified
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port) && int.TryParse(port, out var portInt))
{
    // https://jeremybytes.blogspot.com/2021/11/running-net-6-service-on-specific-port.html
    // https://stackoverflow.com/a/51121731
    builder.WebHost.ConfigureKestrel(opt => opt.ListenAnyIP(portInt));
}

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(config =>
{
    config.UseInMemoryDatabase("TestInMemoryUsersDb");
});

builder.Services.AddIdentity<User, IdentityRole>(config =>
{
    config.Password.RequiredLength = 6;
    config.Password.RequireDigit = false;
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequireUppercase = false;
    config.User.RequireUniqueEmail = true;
    config.SignIn.RequireConfirmedEmail = 
        builder.Configuration.GetRequiredSection("Auth")
        .Get<AuthServiceSettings>().RequireConfirmedEmailToLogin;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddGenericRepositories();
builder.Services.AddAutoMapper(config =>
{
    config.AddMaps(Assembly.GetExecutingAssembly());
});

builder.Services.AddJwtBearerAuth();
builder.Services.AddMailKitEmailConfirmation();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddJwtBearerAuthSwaggerUI();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("EnforceSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

app.Run();
