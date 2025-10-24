using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Pangolivia.API.Options;
using Pangolivia.API.Services;
using Pangolivia.API.DTOs;
using Xunit;

public class AiQuizServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly AiQuizService _aiQuizService;

    public AiQuizServiceTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        // Mock OpenAiOptions
        var options = Options.Create(new OpenAiOptions
        {
            ApiKey = "fake-api-key",
            Model = "gpt-4.1-mini",
            Temperature = 0.5
        });

        // Mock HttpClient with properly serialized JSON
        var handlerMock = new Mock<HttpMessageHandler>();
        var aiResponse = new
        {
            choices = new[]
            {
                new
                {
                    message = new
                    {
                        content = JsonSerializer.Serialize(new[]
                        {
                            new
                            {
                                questionText = "What is 2+2?",
                                options = new[] { "3", "4", "5", "6" },
                                correctOptionIndex = 1
                            }
                        })
                    }
                }
            }
        };

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(aiResponse), Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://api.openai.com/")
        };

        _httpClientFactoryMock.Setup(f => f.CreateClient("OpenAI")).Returns(httpClient);

        // Mock logger
        var loggerMock = new Mock<ILogger<AiQuizService>>();

        _aiQuizService = new AiQuizService(_httpClientFactoryMock.Object, options, loggerMock.Object);
    }

    [Fact]
    public async Task GenerateQuestionsAsync_ReturnsQuestions()
    {
        // Arrange
        string topic = "Math";
        int numberOfQuestions = 1;
        string difficulty = "Easy";

        // Act
        var result = await _aiQuizService.GenerateQuestionsAsync(topic, numberOfQuestions, difficulty);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        var question = result[0];
        Assert.Equal("What is 2+2?", question.QuestionText);
        Assert.Equal("4", question.CorrectAnswer); // correctOptionIndex = 1
        Assert.Equal("5", question.Answer2);       // rotated answers
        Assert.Equal("6", question.Answer3);
        Assert.Equal("3", question.Answer4);
    }
}
