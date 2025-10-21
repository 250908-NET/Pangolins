using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.Services;
using Pangolivia.API.DTOs;

namespace Pangolivia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerGameRecordController : ControllerBase
    {
        private readonly IPlayerGameRecordService _playerGameRecordService;

        public PlayerGameRecordController(IPlayerGameRecordService playerGameRecordService)
        {
            _playerGameRecordService = playerGameRecordService;
        }

        // POST: api/PlayerGameRecord
        // Record a player's score after finishing a quiz
        [HttpPost]
        public async Task<IActionResult> RecordScore([FromBody] CreatePlayerGameRecordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _playerGameRecordService.RecordScoreAsync(dto);
                return CreatedAtAction(nameof(GetPlayerHistory), new { userId = result.UserId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/PlayerGameRecord/leaderboard/{gameRecordId}
        // Get the leaderboard for a given game
        [HttpGet("leaderboard/{gameRecordId}")]
        public async Task<IActionResult> GetLeaderboard(int gameRecordId)
        {
            var leaderboard = await _playerGameRecordService.GetLeaderboardAsync(gameRecordId);
            return Ok(leaderboard);
        }

        // 📜 GET: api/PlayerGameRecord/history/{userId}
        // Get player's history of game scores
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetPlayerHistory(int userId)
        {
            var history = await _playerGameRecordService.GetPlayerHistoryAsync(userId);
            return Ok(history);
        }

        // GET: api/PlayerGameRecord/average/{userId}
        // Get player's average score
        [HttpGet("average/{userId}")]
        public async Task<IActionResult> GetAverageScore(int userId)
        {
            var average = await _playerGameRecordService.GetAverageScoreByPlayerAsync(userId);
            return Ok(new { userId, averageScore = average });
        }

        // PUT: api/PlayerGameRecord/{recordId}
        // Update a player's score
        [HttpPut("{recordId}")]
        public async Task<IActionResult> UpdateScore(int recordId, [FromBody] UpdatePlayerGameRecordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _playerGameRecordService.UpdateScoreAsync(recordId, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/PlayerGameRecord/{recordId}
        // Delete a player's game record
        [HttpDelete("{recordId}")]
        public async Task<IActionResult> DeleteRecord(int recordId)
        {
            try
            {
                await _playerGameRecordService.DeleteRecordAsync(recordId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
