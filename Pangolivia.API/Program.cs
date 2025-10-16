using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;  // ok to keep even if unused right now
using AutoMapper;
using Serilog;
using Pangolivia.API.Api.Middleware;

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
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

// Register EF Core context (single registration)
builder.Services.AddDbContext<PangoliviaDbContext>(opt =>
    opt.UseSqlServer(conn));

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

var app = builder.Build();

// Global exception handler (keep early)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
