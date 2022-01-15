using PollutionMapAPI.DTOs.Entities;
using PollutionMapAPI.Models;

namespace PollutionMapAPI.Services.Map;

public interface IMapService
{
    public Task<Models.Map?> CreateMapAsync(User user, MapRequestDTO value);
    public Task<Models.Map?> GetMapByIdAsync(Guid mapId);
    public Task<Models.Map?> UpdateMapAsync(User user, Guid mapId, MapRequestDTO value);
    public Task<bool> DeleteMapAsync(User user, Guid mapId);
    public Task<IEnumerable<Models.Map>> GetAllMapsAsync(User user);

}
