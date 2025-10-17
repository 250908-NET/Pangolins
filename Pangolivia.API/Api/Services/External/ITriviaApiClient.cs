using System.Text.Json.Serialization;

namespace Pangolivia.API.Services.External
{
    public interface ITriviaApiClient
    {
        Task<OpenTriviaResponse> FetchAsync(int amount = 5, int? category = null, string? difficulty = null, string type = "multiple", CancellationToken ct = default);
    }

    // --- DTOs for Open Trivia DB ---
    public class OpenTriviaResponse
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("results")]
        public List<OpenTriviaResult> Results { get; set; } = new();
    }

    public class OpenTriviaResult
    {
        [JsonPropertyName("category")]
        public string Category { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; } = "";

        [JsonPropertyName("question")]
        public string Question { get; set; } = "";

        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; set; } = "";

        [JsonPropertyName("incorrect_answers")]
        public List<string> IncorrectAnswers { get; set; } = new();
    }
}
