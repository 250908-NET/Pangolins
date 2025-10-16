namespace Pangolivia.API.DTOs
{
    public class CreateGameRecordDto
    {
        public int HostUserId { get; set; }
        public int QuizId { get; set; }
    }

    public class GameRecordDto
    {
        public int Id { get; set; }
        public int HostUserId { get; set; }
        public int QuizId { get; set; }
        public string? QuizName { get; set; }
        public DateTime datetimeCompleted { get; set; }
    }
}
