using System;


namespace Pangolivia.API.DTOs
{
public class GameRecordDTO
    {
        public int Id { get; set; }
        public int HostUserId { get; set; }
        public int QuizId { get; set; }
        public DateTime DatetimeCompleted { get; set; }
    }
}