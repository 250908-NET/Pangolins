namespace Pangolivia.API.DTOs
{
    public class UserDetailDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public List<QuizSummaryDto> CreatedQuizzes { get; set; } = new();
        public int HostedGamesCount { get; set; }
        public int GamesPlayedCount { get; set; }
    }
}
