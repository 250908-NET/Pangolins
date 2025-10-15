using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pangolivia.Models;

[Table("quiz")]
public class Quiz
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string QuizName { get; set; } = string.Empty;

    [ForeignKey("User")]
    public int CreatedByUserId { get; set; }

    // Links to other models
    public User? CreatedByUser { get; set; }
    public List<Question> Questions { get; set; } = new();
}

