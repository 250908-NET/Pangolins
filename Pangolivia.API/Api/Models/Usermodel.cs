using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pangolivia.API.Models;

[Table("users")]
public class UserModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("auth_sub")]
    public string Auth0Sub { get; set; } = string.Empty;

    [Required]
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    public string ProfileImageUrl { get; set; } = string.Empty;

    public ICollection<PlayerGameRecordModel> PlayerGameRecords { get; set; } =
        new List<PlayerGameRecordModel>();
    public ICollection<GameRecordModel> HostedGameRecords { get; set; } =
        new List<GameRecordModel>();
    public ICollection<QuizModel> CreatedQuizzes { get; set; } = new List<QuizModel>();
}
