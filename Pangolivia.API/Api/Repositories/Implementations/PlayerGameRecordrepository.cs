using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.Models;

public class PlayerGameRecordRepository : IPlayerGameRecordRepository
{
    private PangoliviaDbContext _context;
    PlayerGameRecordRepository(PangoliviaDbContext context)
    {
        _context = context;
    }
    public async Task<List<PlayerGameRecordModel>> getAllPlayerGameRecords()
    {
        return await _context.PlayerGameRecords
         .Include(pgr => pgr.User)
         .Include(pgr => pgr.GameRecord)
         .ToListAsync();
    }
    public async Task<PlayerGameRecordModel> getPlayerGameRecordModelByUserId(int userId)
    {
        var FoundPlayerGameRecordModel = await _context.PlayerGameRecords
        .Include(pgr => pgr.GameRecord)
        .FirstOrDefaultAsync(pgr => pgr.UserId == userId);

        if (FoundPlayerGameRecordModel != null)
        {
            return FoundPlayerGameRecordModel;
        }
        return null;
        // throw new NotImplementedException();
    }
    public async Task<PlayerGameRecordModel> AddPlayerGameRecordModel(PlayerGameRecordDto PGRM_DTO)
    {
        var model = new PlayerGameRecordModel
        {
            UserId = PGRM_DTO.UserId,
            GameRecordId = PGRM_DTO.GameRecordId,
            score = PGRM_DTO.Score
        };
        _context.PlayerGameRecords.Add(model);
        await _context.SaveChangesAsync();
        return model;
    }
    public async Task RemovePlayerGameRecordModel(int id)
    {
        var record = await _context.PlayerGameRecords.FindAsync(id);

        if (record == null)
        {
            throw new KeyNotFoundException($"PlayerGameRecord with id {id} not found.");
        }

        _context.PlayerGameRecords.Remove(record);
        await _context.SaveChangesAsync();

    }
    public async Task UpdatePlayerGameRecordModel(int id, int newScore)
    {
        var record = await _context.PlayerGameRecords.FindAsync(id);

        if (record == null)
        {
            throw new KeyNotFoundException($"PlayerGameRecord with id {id} not found.");
        }

        record.score = newScore;
        await _context.SaveChangesAsync();
    }
}