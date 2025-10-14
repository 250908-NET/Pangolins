namespace  Pangolivia.Models;

public class GameRecord  {
    [key]
    int id {get; set;}

    [ForeignKey(nameof())]
    int hostUserId {get; set;}
    
    [ForeignKey (nameof())]
    int quizId {get; set;} 

    Datetime datetimeCompleted {get; set;} 

    public ICollection<PlayerGameRecord> PlayerRecordList { get; set; } = new List<PlayerGameRecord>();
}