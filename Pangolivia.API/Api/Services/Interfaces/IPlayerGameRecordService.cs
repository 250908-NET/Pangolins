using Pangolivia.API.DTOs;

namespace Pangolivia.API.Services.Interfaces
{
    public interface IPlayerGameRecordService
    {
        Task<PlayerGameRecordDto> RecordScoreAsync(CreatePlayerGameRecordDto dto);
        Task<IEnumerable<LeaderboardDto>> GetLeaderboardAsync(int gameRecordId);
        Task<IEnumerable<PlayerGameRecordDto>> GetPlayerHistoryAsync(int userId);
        Task<double> GetAverageScoreByPlayerAsync(int userId);
        Task UpdateScoreAsync(int recordId, UpdatePlayerGameRecordDto dto);
        Task DeleteRecordAsync(int recordId);
    }
}
