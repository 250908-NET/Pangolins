using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pangolivia.API.Models;

[Table("quizzes")]
public class QuizModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("quiz_name")]
    public string QuizName { get; set; } = string.Empty;

    [Required]
    [Column("created_by_user_id")]
    public int CreatedByUserId { get; set; }

    [ForeignKey(nameof(CreatedByUserId))]
    public UserModel? CreatedByUser { get; set; }

    public ICollection<QuestionModel> Questions { get; set; } = new List<QuestionModel>();
    public ICollection<GameRecordModel> GameRecords { get; set; } = new List<GameRecordModel>();
}

