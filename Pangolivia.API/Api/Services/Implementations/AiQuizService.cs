using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pangolivia.API.DTOs;
using Pangolivia.API.Options;

namespace Pangolivia.API.Services;

public class AiQuizService : IAiQuizService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OpenAiOptions _options;
    private readonly ILogger<AiQuizService> _logger;

    public AiQuizService(
        IHttpClientFactory httpClientFactory,
        IOptions<OpenAiOptions> options,
        ILogger<AiQuizService> logger
    )
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Generates quiz questions using OpenAI's API based on the specified topic, count, and difficulty.
    /// </summary>
    public async Task<List<QuestionDto>> GenerateQuestionsAsync(
        string topic,
        int numberOfQuestions,
        string difficulty,
        CancellationToken cancellationToken = default
    )
    {
        // Create HTTP client configured for OpenAI API
        var httpClient = _httpClientFactory.CreateClient("OpenAI");

        // Construct prompt instructing AI to generate structured quiz questions
        var userPrompt =
            $"Generate {numberOfQuestions} {difficulty} difficulty 4-option multiple-choice questions about '{topic}'. Output strictly as JSON array of objects with fields: questionText, options (exactly 4 strings), correctOptionIndex (0-3). No explanations.";

        // Build OpenAI Chat Completions API request payload
        var openAiRequestPayload = new
        {
            model = string.IsNullOrWhiteSpace(_options.Model) ? "gpt-4.1-mini" : _options.Model,
            temperature = _options.Temperature,
            messages = new object[]
            {
                new
                {
                    role = "system",
                    content = "You write multiple-choice quiz questions. Return only valid JSON.",
                },
                new { role = "user", content = userPrompt },
            },
        };

        // Prepare HTTP request with authorization and JSON payload
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/chat/completions");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            _options.ApiKey
        );
        httpRequest.Content = new StringContent(
            JsonSerializer.Serialize(openAiRequestPayload),
            Encoding.UTF8,
            "application/json"
        );

        // Send request to OpenAI and ensure successful response
        var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        // Parse OpenAI response and extract the AI-generated content
        var responseJson = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        using var jsonDocument = JsonDocument.Parse(responseJson);
        var aiGeneratedContent =
            jsonDocument
                .RootElement.GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
            ?? "[]";

        // Extract JSON array from AI response (handles cases where AI adds extra text)
        var jsonArrayText = TryExtractJsonArray(aiGeneratedContent);

        // Deserialize AI response into strongly-typed question items
        var aiQuestionItems =
            JsonSerializer.Deserialize<List<AiQuestionItem>>(
                jsonArrayText,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<AiQuestionItem>();

        // Validate and convert AI questions to QuestionDto format
        var validatedQuestions = new List<QuestionDto>();
        foreach (var aiQuestion in aiQuestionItems)
        {
            // Skip null or invalid questions
            if (aiQuestion == null)
                continue;
            if (aiQuestion.Options == null || aiQuestion.Options.Count != 4)
                continue;

            // Validate correct answer index is within valid range (0-3)
            var correctAnswerIndex = aiQuestion.CorrectOptionIndex;
            if (correctAnswerIndex < 0 || correctAnswerIndex > 3)
                continue;

            // Add validated question to result list
            validatedQuestions.Add(
                new QuestionDto
                {
                    QuestionText = aiQuestion.QuestionText ?? string.Empty,
                    CorrectAnswer = aiQuestion.Options[correctAnswerIndex],
                    Answer2 = aiQuestion.Options[(correctAnswerIndex + 1) % 4],
                    Answer3 = aiQuestion.Options[(correctAnswerIndex + 2) % 4],
                    Answer4 = aiQuestion.Options[(correctAnswerIndex + 3) % 4],
                }
            );
        }

        return validatedQuestions;
    }

    /// <summary>
    /// Extracts a JSON array from text that may contain additional content.
    /// Handles cases where the AI response includes explanatory text before/after the JSON.
    /// </summary>
    private static string TryExtractJsonArray(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "[]";

        // Find the first '[' and last ']' to extract the JSON array
        var arrayStartIndex = text.IndexOf('[');
        var arrayEndIndex = text.LastIndexOf(']');

        if (arrayStartIndex >= 0 && arrayEndIndex > arrayStartIndex)
        {
            return text.Substring(arrayStartIndex, arrayEndIndex - arrayStartIndex + 1);
        }

        // Return empty array if no valid JSON array found
        return "[]";
    }

    /// <summary>
    /// Internal model for deserializing AI-generated question data from OpenAI response.
    /// Maps to the JSON structure requested in the prompt.
    /// </summary>
    private class AiQuestionItem
    {
        [JsonPropertyName("questionText")]
        public string? QuestionText { get; set; }

        [JsonPropertyName("options")]
        public List<string> Options { get; set; } = new();

        [JsonPropertyName("correctOptionIndex")]
        public int CorrectOptionIndex { get; set; }
    }
}
