using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pangolivia.API.Models;

[Table("game_records")]
public class GameRecordModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("host_user_id")]
    public int HostUserId { get; set; }
    [ForeignKey(nameof(HostUserId))]
    public UserModel? HostUser { get; set; }

    [Required]
    [Column("quiz_id")]
    public int QuizId { get; set; }
    [ForeignKey(nameof(QuizId))]
    public QuizModel? Quiz { get; set; }

    [Required]
    [Column("datetime_completed")]
    public DateTime datetimeCompleted { get; set; }

    // public ICollection<PlayerGameRecordModel> PlayerGameRecords { get; set; } = new List<PlayerGameRecordModel>();
}