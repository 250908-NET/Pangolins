using Pangolivia.API.Data;
using Pangolivia.API.Models;

public class PlayerGameRecordRepository : IPlayerGameRecordRepository
{
    private PangoliviaDbContext _context;
    PlayerGameRecordRepository(PangoliviaDbContext context)
    {
        _context = context;
    }
    public Task<List<PlayerGameRecordModel>> getAllPlayerGameRecords()
    {
        throw new NotImplementedException();
    }
    public Task<PlayerGameRecordModel> getPlayerGameRecordModelByUserId(int userID)
    {
        throw new NotImplementedException();
    }
    public Task<PlayerGameRecordModel> AddPlayerGameRecordModel(PlayerGameRecordModel PGRM_DTO)
    {
        throw new NotImplementedException();
    }
    public Task RemovePlayerGameRecordModel(int id)
    {
        throw new NotImplementedException();
    }
    public Task UpdatePlayerGameRecordModel(int id)
    {
        throw new NotImplementedException();
    }
}