using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pangolivia.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public string CorrectAnswer { get; set; } = string.Empty;

        [Required]
        public string Answer2 { get; set; } = string.Empty;

        [Required]
        public string Answer3 { get; set; } = string.Empty;

        [Required]
        public string Answer4 { get; set; } = string.Empty;

        
        public Quiz? Quiz { get; set; }
    }
}
