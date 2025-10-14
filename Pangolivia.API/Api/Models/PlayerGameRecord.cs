using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Pangolivia.Models;

public class PlayerGameRecord
{

    [Key]
    int id { get; set; }

    [ForeignKey(nameof(UserId))]
    int userId { get; set; }


    [Required]
    double score { get; set; } = 0;


    // remove after merge
    private object UserId()
    {
        throw new NotImplementedException();
    }

}