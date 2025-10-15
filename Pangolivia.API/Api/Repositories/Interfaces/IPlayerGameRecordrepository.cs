using Pangolivia.API.Models;

public interface IPlayerGameRecordRepository
{
    public Task<List<PlayerGameRecordModel>> getAllPlayerGameRecords();
    public Task<PlayerGameRecordModel> getPlayerGameRecordModelByUserId(int userID);
    public Task<PlayerGameRecordModel> AddPlayerGameRecordModel(PlayerGameRecordDto PGRM_DTO);
    public Task RemovePlayerGameRecordModel(int id);
    public Task UpdatePlayerGameRecordModel(int id, int newScore);


}