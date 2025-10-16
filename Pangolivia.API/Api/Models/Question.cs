using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Placeholder

namespace Pangolivia.API.Models;

[Table("question")]
public class Question
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(300)]
    [Column("question_text")]
    public string QuestionText { get; set; } = string.Empty;

    // Foreign key to Quiz
    [ForeignKey("Quiz")]
    [Column("quiz_id")]
    public int QuizId { get; set; }

    // Navigation property
    public QuizModel? QuizModel { get; set; }
}

