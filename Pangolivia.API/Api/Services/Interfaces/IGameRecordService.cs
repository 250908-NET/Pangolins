using Pangolivia.API.DTOs;

namespace Pangolivia.API.Services
{
    public interface IGameRecordService
    {
        Task<GameRecordDto> CreateGameAsync(CreateGameRecordDto dto);
        Task<IEnumerable<GameRecordDto>> GetAllGamesAsync();
        Task<GameRecordDto?> GetGameByIdAsync(int id);
        Task<GameRecordDto?> CompleteGameAsync(int gameId);
        Task DeleteGameAsync(int gameId);
        Task<IEnumerable<GameRecordDto>> GetGamesByHostAsync(int hostUserId);
        Task<IEnumerable<GameRecordDto>> GetGamesByQuizAsync(int quizId);
    }
}
