using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;
using Pangolivia.API.Models;
using Pangolivia.API.Middleware;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Pangolivia.API.Options;
using System;


var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.EnvironmentName == "Development")
{
    DotNetEnv.Env.Load();
}
// builder.Configuration
//     .SetBasePath(Directory.GetCurrentDirectory())
//     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
//     .AddEnvironmentVariables(); // Reads both system and Docker environment variables

// Print connection string (try ConnectionStrings:Connection, then "Connection" key, then environment variable)
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Connection") ?? "N/A";
Console.WriteLine($"Connection String: {connectionString}");

// Print all environment variables containing "Connection" (case-insensitive)
var envVars = Environment.GetEnvironmentVariables();
Console.WriteLine("Environment variables containing 'Connection':");
foreach (System.Collections.DictionaryEntry entry in envVars)
{
    var key = entry.Key?.ToString() ?? "";
    if (key.IndexOf("Connection", StringComparison.OrdinalIgnoreCase) >= 0)
    {
        var value = entry.Value?.ToString() ?? "";
        Console.WriteLine($"{key} = {value}");
    }
}

// Register DbContext with the read connection string
builder.Services.AddDbContext<PangoliviaDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("Connection") ?? "";
    if (connectionString == "")
    {
        Console.WriteLine("Connection string not found. Exiting program.");
        // Environment.Exit(1);
    }
    options.UseSqlServer(connectionString);
});

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IGameRecordRepository, GameRecordRepository>();
builder.Services.AddScoped<IPlayerGameRecordRepository, PlayerGameRecordRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IGameRecordService, GameRecordService>();
builder.Services.AddScoped<IPlayerGameRecordService, PlayerGameRecordService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAiQuizService, AiQuizService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// OpenAI configuration & services
builder.Services.Configure<OpenAiOptions>(builder.Configuration.GetSection("OpenAI"));
builder.Services.PostConfigure<OpenAiOptions>(o =>
{
    if (string.IsNullOrWhiteSpace(o.ApiKey))
    {
        o.ApiKey = builder.Configuration["OPENAI_API_KEY"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty;
    }
});
builder.Services.AddHttpClient("OpenAI", c =>
{
    c.BaseAddress = new Uri("https://api.openai.com/");
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMiddleware<RequestLoggingMiddleware>();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

// Seed data at startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PangoliviaDbContext>();

    // Apply migrations automatically
    // context.Database.Migrate();

    // Check if the tables are empty
    if (!context.Users.Any() && !context.Quizzes.Any())
    {
        // Create a user
        var user = new UserModel
        {
            AuthUuid = Guid.NewGuid().ToString(),
            Username = "testadmin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
        };
        context.Users.Add(user);
        context.SaveChanges();

        // Create a quiz
        var quiz = new QuizModel
        {
            QuizName = "Test Quiz",
            CreatedByUserId = user.Id
        };
        context.Quizzes.Add(quiz);
        context.SaveChanges();

        // Create 5 sample questions
        var questions = new List<QuestionModel>
        {
            new QuestionModel
            {
                QuizId = quiz.Id,
                QuestionText = "What is the capital of France?",
                CorrectAnswer = "Paris",
                Answer2 = "London",
                Answer3 = "Berlin",
                Answer4 = "Madrid"
            },
            
            new QuestionModel
            {
                QuizId = quiz.Id,
                QuestionText = "Which planet is known as the Red Planet?",
                CorrectAnswer = "Mars",
                Answer2 = "Venus",
                Answer3 = "Jupiter",
                Answer4 = "Saturn"
            },
            new QuestionModel
            {
                QuizId = quiz.Id,
                QuestionText = "What is the primary goal of the player at the start of Stardew Valley?",
                CorrectAnswer = "Restore and manage a neglected farm inherited from their grandfather",
                Answer2 = "Build the largest house in Pelican Town",
                Answer3 = "Defeat monsters in the Skull Cavern",
                Answer4 = "Become the mayor of Stardew Valley"
            },
            new QuestionModel
            {
                QuizId = quiz.Id,
                QuestionText = "What is the largest ocean on Earth?",
                CorrectAnswer = "Pacific Ocean",
                Answer2 = "Atlantic Ocean",
                Answer3 = "Indian Ocean",
                Answer4 = "Arctic Ocean"
            },
            new QuestionModel
            {
                QuizId = quiz.Id,
                QuestionText = "What is the chemical symbol for Gold?",
                CorrectAnswer = "Au",
                Answer2 = "Ag",
                Answer3 = "Fe",
                Answer4 = "Cu"
            }
        };

        context.Questions.AddRange(questions);
        context.SaveChanges();
    }
}

app.Run();
