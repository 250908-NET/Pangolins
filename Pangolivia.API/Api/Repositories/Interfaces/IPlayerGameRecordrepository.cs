using Pangolivia.API.Models;

public interface IPlayerGameRecordRepository
{
    public Task<List<PlayerGameRecordModel>> getAllPlayerGameRecords();
    public Task<PlayerGameRecordModel> getPlayerGameRecordModelByUserId(int userID);
    public Task<PlayerGameRecordModel> AddPlayerGameRecordModel(PlayerGameRecordModel PGRM_DTO);
    public Task RemovePlayerGameRecordModel(int id);
    public Task UpdatePlayerGameRecordModel(int id);


}