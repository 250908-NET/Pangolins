using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;

var builder = WebApplication.CreateBuilder(args);

//  Configuration hierarchy: appsettings → secrets → env vars
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables();

//  Read the connection string
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

//  Register EF Core context
builder.Services.AddDbContext<PangoliviaDbContext>(opt =>
    opt.UseSqlServer(conn));


// Read connection string from text file
// var connectionStringPath = Path.Combine(Directory.GetCurrentDirectory(), "ConnectionString.txt");
// if (!File.Exists(connectionStringPath))
// {
//     throw new FileNotFoundException("Could not find ConnectionString.txt at project root.", connectionStringPath);
// }
// var connectionString = File.ReadAllText(connectionStringPath).Trim();

// Register DbContext with the read connection string
builder.Services.AddDbContext<PangoliviaDbContext>(options =>
    options.UseSqlServer(conn));

// Dependency Injection
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

// builder.Services.AddScoped<IQuizService, QuizService>();

// AutoMapper
// builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
