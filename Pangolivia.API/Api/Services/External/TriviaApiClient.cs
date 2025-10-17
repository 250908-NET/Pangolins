using System.Net.Http.Json;
using System.Web;

namespace Pangolivia.API.Services.External
{
    public class TriviaApiClient : ITriviaApiClient
    {
        private readonly HttpClient _http;

        public TriviaApiClient(HttpClient http)
        {
            _http = http;
            if (_http.BaseAddress == null)
            {
                _http.BaseAddress = new Uri("https://opentdb.com/");
            }
        }

        public async Task<OpenTriviaResponse> FetchAsync(int amount = 5, int? category = null, string? difficulty = null, string type = "multiple", CancellationToken ct = default)
        {
            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["amount"] = amount.ToString();
            if (category.HasValue) qs["category"] = category.Value.ToString();
            if (!string.IsNullOrWhiteSpace(difficulty)) qs["difficulty"] = difficulty;
            if (!string.IsNullOrWhiteSpace(type)) qs["type"] = type;

            var uri = new Uri($"api.php?{qs}", UriKind.Relative);
            var resp = await _http.GetFromJsonAsync<OpenTriviaResponse>(uri, cancellationToken: ct);
            return resp ?? new OpenTriviaResponse();
        }
    }
}
