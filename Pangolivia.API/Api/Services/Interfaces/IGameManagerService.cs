using Pangolivia.API.DTOs;
using Pangolivia.API.Models;

namespace Pangolivia.API.Services;

public interface IGameManagerService
{
    Task<string> CreateGame(int quizId, int hostUserId);
    GameSession? GetGameSession(string roomCode);
}

