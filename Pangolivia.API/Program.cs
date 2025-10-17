using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;
using Pangolivia.API.Models;
using Pangolivia.API.Middleware;


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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

// Services
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

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
    app.UseMiddleware<RequestLoggingMiddleware>();
}

app.UseHttpsRedirection();
//app.UseAuthorization();
app.UseCors();
app.MapControllers();

// Seed data at startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PangoliviaDbContext>();

    // Apply migrations automatically
    context.Database.Migrate();

    // Check if the tables are empty
    if (!context.Users.Any() && !context.Quizzes.Any())
    {
        // Create a user
        var user = new UserModel
        {
            AuthUuid = Guid.NewGuid().ToString(),
            Username = "testadmin"
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
