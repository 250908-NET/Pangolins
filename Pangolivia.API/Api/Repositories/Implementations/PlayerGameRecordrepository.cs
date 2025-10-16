using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.Models;

public class PlayerGameRecordRepository : IPlayerGameRecordRepository
{
    private PangoliviaDbContext _context;
    public PlayerGameRecordRepository(PangoliviaDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<PlayerGameRecordModel>> GetAllAsync()
        {
            return await _context.PlayerGameRecords
                .Include(p => p.User)
                .Include(p => p.GameRecord)
                .ToListAsync();
        }

        public async Task<PlayerGameRecordModel?> GetByIdAsync(int id)
        {
            return await _context.PlayerGameRecords
                .Include(p => p.User)
                .Include(p => p.GameRecord)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PlayerGameRecordModel>> GetByUserIdAsync(int userId)
        {
            return await _context.PlayerGameRecords
                .Where(p => p.UserId == userId)
                .Include(p => p.GameRecord)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerGameRecordModel>> GetByGameRecordIdAsync(int gameRecordId)
        {
            return await _context.PlayerGameRecords
                .Where(p => p.GameRecordId == gameRecordId)
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<PlayerGameRecordModel> AddAsync(PlayerGameRecordModel record)
        {
            _context.PlayerGameRecords.Add(record);
            await _context.SaveChangesAsync();
            return record;
        }

        public async Task<PlayerGameRecordModel> UpdateAsync(PlayerGameRecordModel record)
        {
            _context.PlayerGameRecords.Update(record);
            await _context.SaveChangesAsync();
            return record;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var record = await _context.PlayerGameRecords.FindAsync(id);
            if (record == null)
                return false;

            _context.PlayerGameRecords.Remove(record);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<double> GetAverageScoreByGameAsync(int gameRecordId)
        {
            return await _context.PlayerGameRecords
                .Where(p => p.GameRecordId == gameRecordId)
                .AverageAsync(p => p.score);
        }
}