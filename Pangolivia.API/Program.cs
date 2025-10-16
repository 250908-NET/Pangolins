using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register DbContext with the read connection string
builder.Services.AddDbContext<PangoliviaDbContext>(options =>
    options.UseSqlServer(connectionString));

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
