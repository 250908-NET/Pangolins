using System.Collections.Generic;
using System.Threading.Tasks;
using Pangolivia.API.Models;

namespace Pangolivia.Repositories
{
    public interface IPlayerGameRecordRepository
    {
        Task<IEnumerable<PlayerGameRecordModel>> GetAllAsync();
        Task<PlayerGameRecordModel?> GetByIdAsync(int id);
        Task<IEnumerable<PlayerGameRecordModel>> GetByUserIdAsync(int userId);
        Task<IEnumerable<PlayerGameRecordModel>> GetByGameRecordIdAsync(int gameRecordId);
        Task<PlayerGameRecordModel> AddAsync(PlayerGameRecordModel record);
        Task<PlayerGameRecordModel> UpdateAsync(PlayerGameRecordModel record);
        Task<bool> DeleteAsync(int id);
        Task<double> GetAverageScoreByGameAsync(int gameRecordId);
    }
}