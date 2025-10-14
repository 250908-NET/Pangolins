using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pangolivia.Models;
public class GameRecord
{
    [Key]
    int id { get; set; }

    [ForeignKey(nameof(User))]
    int hostUserId { get; set; }


    [ForeignKey(nameof(QuizId))]
    int quizId { get; set; }


    DateTime datetimeCompleted { get; set; }

    public ICollection<PlayerGameRecord> PlayerRecordList { get; set; } = new List<PlayerGameRecord>();

    //    remove after merge
    private object QuizId()
    {
        throw new NotImplementedException();
    }
    private object User()
    {
        throw new NotImplementedException();
    }
}