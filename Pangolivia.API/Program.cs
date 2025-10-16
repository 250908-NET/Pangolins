using Microsoft.EntityFrameworkCore;
using Pangolivia.Data;
using Pangolivia.Repositories;
using Pangolivia.Services;

var builder = WebApplication.CreateBuilder(args);

// Read connection string from text file
var connectionStringPath = Path.Combine(Directory.GetCurrentDirectory(), "ConnectionString.txt");
if (!File.Exists(connectionStringPath))
{
    throw new FileNotFoundException("Could not find ConnectionString.txt at project root.", connectionStringPath);
}
var connectionString = File.ReadAllText(connectionStringPath).Trim();

// Register DbContext with the read connection string
builder.Services.AddDbContext<PangoliviaDbContext>(options =>
    options.UseSqlServer(connectionString));

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
