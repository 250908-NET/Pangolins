namespace  Pangolivia.Models;

public class PlayerGameRecord {

    [key]
    int id {get; set;}
    
    [ForeignKey (nameof())]
    int userId {get; set;}

    [required]
    double score {get; set;} = 0;


}