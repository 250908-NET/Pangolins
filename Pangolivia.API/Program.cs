using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;  // ok to keep even if unused right now
using AutoMapper;
using Serilog;
using Pangolivia.API.Api.Middleware;
using Pangolivia.API.Services.External;


var builder = WebApplication.CreateBuilder(args);

// Serilog: console + rolling file (logs/log-.txt)
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configuration hierarchy: appsettings → secrets → env vars
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables();

// Read the connection string
// var conn = builder.Configuration.GetConnectionString("DefaultConnection");

// Register EF Core context (single registration)
// builder.Services.AddDbContext<PangoliviaDbContext>(opt =>
//     opt.UseSqlServer(conn));
// Read the connection string
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

// Register EF Core context with resilient retries
builder.Services.AddDbContext<PangoliviaDbContext>(opt =>
    opt.UseSqlServer(conn, sql =>
    {
        sql.EnableRetryOnFailure(
            maxRetryCount: 5,                         // number of retries
            maxRetryDelay: TimeSpan.FromSeconds(10),  // backoff between retries
            errorNumbersToAdd: null);                 // let EF choose transient errors
        // Optional: increase command timeout if your DB is slow to wake up
        // sql.CommandTimeout(120);
    }));


// Dependency Injection
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Exception middleware DI
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

// Trivia API client
builder.Services.AddHttpClient<ITriviaApiClient, TriviaApiClient>(client =>
{
    client.BaseAddress = new Uri("https://opentdb.com/");
});


var app = builder.Build();

// Run migrations at startup (safe with retries enabled)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PangoliviaDbContext>();
    db.Database.Migrate();
}


// Global exception handler (keep early)
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
// Swagger UI in Development and Production only
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
