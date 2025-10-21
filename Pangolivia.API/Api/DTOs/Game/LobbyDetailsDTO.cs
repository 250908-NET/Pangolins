namespace Pangolivia.API.DTOs
{
    public class LobbyDetailsDto
    {
        public string QuizName { get; set; } = string.Empty;
        public string CreatorUsername { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
    }
}
