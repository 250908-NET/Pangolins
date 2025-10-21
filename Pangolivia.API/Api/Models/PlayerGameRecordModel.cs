using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pangolivia.API.Models;

[Table("player_game_records")]
public class PlayerGameRecordModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserModel? User { get; set; }

    [Required]
    [Column("game_record_id")]
    public int GameRecordId { get; set; }

    [ForeignKey(nameof(GameRecordId))]
    public GameRecordModel? GameRecord { get; set; }

    [Required]
    public double score { get; set; } = 0;
}
