using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.Models;
using Pangolivia.Repositories.Interfaces;

namespace Pangolivia.API.Repositories;

public class GameRecordRepository : IGameRecordRepository
{
    private readonly PangoliviaDbContext _context;
    private readonly ILogger<GameRecordRepository> _logger;

    public GameRecordRepository(PangoliviaDbContext context, ILogger<GameRecordRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<GameRecordModel>> GetAllGameRecordsAsync()
    {
        try
        {
            return await _context.GameRecords.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all game records");
            throw;
        }
    }

    public async Task<GameRecordModel?> GetGameRecordByIdAsync(int gameRecordId)
    {
        try
        {
            return await _context.GameRecords.FindAsync(gameRecordId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving game record with ID {GameRecordId}", gameRecordId);
            throw;
        }
    }

    public async Task<GameRecordModel> CreateGameRecordAsync(GameRecordModel gameRecord)
    {
        try
        {
            gameRecord.datetimeCompleted = DateTime.UtcNow;
            _context.GameRecords.Add(gameRecord);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new game record with ID {GameRecordId}", gameRecord.Id);
            return gameRecord;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating game record");
            throw;
        }
    }

    public async Task<bool> DeleteGameRecordAsync(int gameRecordId)
    {
        try
        {
            var gameRecord = await _context.GameRecords.FindAsync(gameRecordId);
            if (gameRecord == null)
            {
                _logger.LogWarning("Game record with ID {GameRecordId} not found for deletion", gameRecordId);
                return false;
            }

            _context.GameRecords.Remove(gameRecord);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted game record with ID {GameRecordId}", gameRecordId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting game record with ID {GameRecordId}", gameRecordId);
            throw;
        }
    }
}