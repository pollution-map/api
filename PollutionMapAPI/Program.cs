using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.DataAccess;
using PollutionMapAPI.Helpers;
using PollutionMapAPI.Services.Auth;
using PollutionMapAPI.Services.Dataset;
using PollutionMapAPI.Services.Email;
using PollutionMapAPI.Services.Map;
using PollutionMapAPI.Services.UI;
using System.Reflection;
using System.Text.Json.Serialization;

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

builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory());

    // serialize enums as strings in api responses
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var connectionString =
    builder.Configuration.GetConnectionString("DbConnection") ??
    HerokuHelper.GetPostgersConnectionString();

if (builder.Configuration.GetValue<bool>("UseInMemoryDb"))
{
    connectionString = "";
}

builder.Services.AddDbContext<AppDbContext>(config =>
{
    config.UseLazyLoadingProxies();
    if (string.IsNullOrEmpty(connectionString))
        config.UseInMemoryDatabase("TestInMemoryUsersDb");
    else 
        config.UseNpgsql(connectionString, x => x.UseNetTopologySuite());
});

builder.Services.AddIdentity<User, Role>(config =>
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
builder.Services.AddUnitOfWork();
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
    c.IncludeXmlComments(Path.Combine(
        AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"
    ));
});

builder.Services.AddCors();
builder.Services.AddMapService();
builder.Services.AddDatasetService();
builder.Services.AddUIService();

var app = builder.Build();

// Migrate database to latest version on start if needed
if(!string.IsNullOrEmpty(connectionString) && builder.Configuration.GetValue<bool>("MigrateDatabaseOnAppStart"))
    app.Services.MigrateDatabase<AppDbContext>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("EnforceSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global error handler
app.UseExceptionHandler("/error");
app.UseHttpsRedirection();

// allow all origins provided in configuration section "AllowedOrigins"
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").GetChildren().Select(c => c.Value).ToArray();
app.UseCors(x => x
    .WithOrigins(allowedOrigins)
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
