using Pangolivia.API.GameEngine;

namespace Pangolivia.API.Services
{
    public interface IGameManagerService
    {
        Task<string> CreateGame(int quizId, int hostUserId, string hostUsername);
        Task StartGame(string roomCode, int requestingUserId);
        void TriggerGameLoop(string roomCode, int requestingUserId);
        void SubmitAnswer(string roomCode, int userId, string answer);
        void SkipQuestion(string roomCode, int requestingUserId);
        GameSession? GetGameSession(string roomCode);
        bool TryAddPlayerToGame(string roomCode, int userId, string username, string connectionId);
        Task RemovePlayer(string connectionId);
    }
}
