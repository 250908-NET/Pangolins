namespace Pangolivia.API.DTOs
{
    public class QuizDTO
    {
        public int Id { get; set; }
        public string QuizName { get; set; } = string.Empty;
        public int CreatedByUserId { get; set; }
    }
}
