using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.DTOs.Entities;

namespace PollutionMapAPI.Services.Map;

public interface IMapService
{
    public Task<Data.Entities.Map?> CreateMapAsync(Guid userId, MapRequestDTO value);
    public Task<Data.Entities.Map?> GetMapByIdAsync(Guid mapId);
    public Task<Data.Entities.Map?> UpdateMapAsync(Guid userId, Guid mapId, MapRequestDTO value);
    public Task<bool> DeleteMapAsync(Guid userId, Guid mapId);
    public Task<IEnumerable<Data.Entities.Map>> GetAllMapsAsync(Guid userId);

}
