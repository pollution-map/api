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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
