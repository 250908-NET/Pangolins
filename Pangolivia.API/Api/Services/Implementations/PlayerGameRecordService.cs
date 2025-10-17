using Pangolivia.API.DTOs;
using Pangolivia.API.Models;
using Pangolivia.API.Repositories;
using Pangolivia.Repositories.Interfaces;


namespace Pangolivia.API.Services
{
    public class PlayerGameRecordService
    {
        private readonly IPlayerGameRecordRepository _playerGameRecordRepository;
        private readonly IGameRecordRepository _gameRecordRepository;
        private readonly IUserRepository _userRepository;

        public PlayerGameRecordService(
            IPlayerGameRecordRepository playerGameRecordRepository,
            IGameRecordRepository gameRecordRepository,
            IUserRepository userRepository)
        {
            _playerGameRecordRepository = playerGameRecordRepository;
            _gameRecordRepository = gameRecordRepository;
            _userRepository = userRepository;
        }

        // Record a player's score (when they finish a quiz)
        public async Task<PlayerGameRecordDto> RecordScoreAsync(CreatePlayerGameRecordDto dto)
        {
            var game = await _gameRecordRepository.GetGameRecordByIdAsync(dto.GameRecordId);
            var user = await _userRepository.GetByIdAsync(dto.UserId);

            if (game == null)
                throw new Exception($"Game record with ID {dto.GameRecordId} not found.");
            if (user == null)
                throw new Exception($"User with ID {dto.UserId} not found.");

            var record = new PlayerGameRecordModel
            {
                GameRecordId = dto.GameRecordId,
                UserId = dto.UserId,
                score = dto.score
            };

            await _playerGameRecordRepository.AddAsync(record);

            return new PlayerGameRecordDto
            {
                Id = record.Id,
                GameRecordId = record.GameRecordId,
                UserId = record.UserId,
                Username = user.Username,
                score = record.score,
                GameCompletedAt = game.datetimeCompleted
            };
        }

        //Get leaderboard for a game (sorted by score)
        public async Task<IEnumerable<LeaderboardDto>> GetLeaderboardAsync(int gameRecordId)
        {
            var records = await _playerGameRecordRepository.GetByGameRecordIdAsync(gameRecordId);
            var ordered = records
                .OrderByDescending(r => r.score)
                .Select((r, index) => new LeaderboardDto
                {
                    Username = r.User?.Username ?? "Unknown",
                    score = r.score,
                    Rank = index + 1
                });

            return ordered;
        }

        // ✅ Get a player's past game scores
        public async Task<IEnumerable<PlayerGameRecordDto>> GetPlayerHistoryAsync(int userId)
        {
            var records = await _playerGameRecordRepository.GetByUserIdAsync(userId);
            return records.Select(r => new PlayerGameRecordDto
            {
                Id = r.Id,
                GameRecordId = r.GameRecordId,
                UserId = r.UserId,
                Username = r.User?.Username ?? string.Empty,
                score = r.score,
                GameCompletedAt = r.GameRecord?.datetimeCompleted
            });
        }

        // Get player’s average score
        public async Task<double> GetAverageScoreByPlayerAsync(int userId)
        {
            var records = await _playerGameRecordRepository.GetByUserIdAsync(userId);
            if (!records.Any()) return 0;
            return records.Average(r => r.score);
        }

        // Update player’s score 
        public async Task UpdateScoreAsync(int recordId, UpdatePlayerGameRecordDto dto)
        {
            var record = await _playerGameRecordRepository.GetByIdAsync(recordId);
            if (record == null)
                throw new Exception($"PlayerGameRecord with ID {recordId} not found.");

            record.score = dto.score;
            await _playerGameRecordRepository.UpdateAsync(record);
        }

        // Delete player game record 
        public async Task DeleteRecordAsync(int recordId)
        {
            await _playerGameRecordRepository.DeleteAsync(recordId);
        }
    }
}