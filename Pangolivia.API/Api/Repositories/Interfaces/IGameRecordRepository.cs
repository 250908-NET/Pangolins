using Pangolivia.API.Models;

namespace Pangolivia.API.Repositories
{
    public interface IGameRecordRepository
    {
        Task<List<GameRecordModel>> GetAllGameRecordsAsync();
        Task<GameRecordModel?> GetGameRecordByIdAsync(int gameRecordId);
        Task<GameRecordModel> CreateGameRecordAsync(GameRecordModel gameRecord);
        Task<bool> DeleteGameRecordAsync(int gameRecordId);
    }
}
