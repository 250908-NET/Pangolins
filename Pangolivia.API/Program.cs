using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with the read connection string
builder.Services.AddDbContext<PangoliviaDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("Connection") ?? "";
    if (connectionString == "")
    {
        Console.WriteLine("Connection string not found. Exiting program.");
        Environment.Exit(1);
    }
    options.UseSqlServer(connectionString);
});

// Dependency Injection
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

// builder.Services.AddScoped<IQuizService, QuizService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

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
