using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pangolivia.API.Models;

[Table("questions")]
public class QuestionModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("quiz_id")]
    public int QuizId { get; set; }
    [ForeignKey(nameof(QuizId))]
    public QuizModel? Quiz { get; set; }

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
}
