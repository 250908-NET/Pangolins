using Pangolins.Models;

namespace Pangolins.Repositories.Interfaces
{
    public interface IGameRecordRepository
    {
        Task<List<GameRecord>> GetAllGameRecordsAsync();
        Task<GameRecord?> GetGameRecordByIdAsync(int gameRecordId);
        Task<GameRecord> CreateGameRecordAsync(GameRecord gameRecord);
        Task<bool> DeleteGameRecordAsync(int gameRecordId);
    }
}