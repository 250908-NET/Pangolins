using System;
using System.Text;
using System.Threading.Tasks; // Required for Task.CompletedTask
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pangolivia.API.Data;
using Pangolivia.API.Middleware;
using Pangolivia.API.Models;
using Pangolivia.API.Options;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.EnvironmentName == "Development")
{
    DotNetEnv.Env.Load();
}

// Remap Azure SQL connection string to standard format
if (
    Environment.GetEnvironmentVariable("SQLAZURECONNSTR_ConnectionStrings__Connection")
    is string sqlAzureConnStr
)
{
    Environment.SetEnvironmentVariable("ConnectionStrings__Connection", sqlAzureConnStr);
}

// Load configuration values.
builder
    .Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

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

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IGameRecordRepository, GameRecordRepository>();
builder.Services.AddScoped<IPlayerGameRecordRepository, PlayerGameRecordRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IGameRecordService, GameRecordService>();
builder.Services.AddScoped<IPlayerGameRecordService, PlayerGameRecordService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAiQuizService, AiQuizService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
        };

        // *** ADD THIS SECTION TO HANDLE SIGNALR AUTHENTICATION FROM QUERY STRING ***
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/gamehub"))
                {
                    // Read the token from the query string
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            },
        };
    });

// *** DEFINE MULTIPLE CORS POLICIES ***
builder.Services.AddCors(options =>
{
    // Policy for public, anonymous API endpoints. No credentials allowed.
    options.AddPolicy(
        "AllowAnonymous",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );

    // Policy for endpoints that require authentication (JWT token).
    options.AddPolicy(
        "AllowAuthenticated",
        policy =>
        {
            policy
                .WithOrigins([
                    "http://localhost:3000",
                    "https://pangolivia-frontend-gjhpf7gphvhmhgbm.canadacentral-01.azurewebsites.net"
                ]) // Your frontend's specific origin
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials(); // Crucial for sending auth tokens
        }
    );
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
        o.ApiKey =
            builder.Configuration["OPENAI_API_KEY"]
            ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            ?? string.Empty;
    }
});
builder.Services.AddHttpClient(
    "OpenAI",
    c =>
    {
        c.BaseAddress = new Uri("https://api.openai.com/");
    }
);

builder.Services.AddSingleton<GameManagerService>();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors("AllowAuthenticated"); // Use the authenticated policy globally now that SignalR needs it
// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMiddleware<RequestLoggingMiddleware>();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed data at startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PangoliviaDbContext>();
    context.Database.Migrate(); // Applies all pending EF Core migrations

    // Apply migrations automatically
    // context.Database.Migrate();

    //Seed DB
    DbSeeder.Seed(context);
}

// Map the hub
app.MapHub<Pangolivia.API.Hubs.GameHub>("/gamehub");

app.Run();
