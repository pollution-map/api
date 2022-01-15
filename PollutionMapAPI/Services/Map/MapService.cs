using AutoMapper;
using PollutionMapAPI.DTOs.Entities;
using PollutionMapAPI.Models;
using PollutionMapAPI.Repositories.Core;

namespace PollutionMapAPI.Services.Map;

public class MapService : IMapService
{
    private readonly IAsyncRepository<Models.Map, Guid> _mapRepo;
    private readonly IMapper _mapper;

    public MapService(IAsyncRepository<Models.Map, Guid> mapRepo, IMapper mapper)
    {
        _mapRepo = mapRepo;
        _mapper = mapper;
    }

    public async Task<Models.Map?> CreateMapAsync(User user, MapRequestDTO value)
    {
        var map = _mapper.Map<Models.Map>(value);
        map.User = user;
        map.UserId = user.Id;

        await _mapRepo.AddAsync(map);

        return map;
    }

    public Task<Models.Map?> GetMapByIdAsync(Guid mapId)
    {
        return _mapRepo.FirstOrDefaultAsync(x => x.Id == mapId);
    }

    private Task<Models.Map?> GetMapByIdAsync(Guid mapId, User user)
    {
        return _mapRepo.FirstOrDefaultAsync(x => x.Id == mapId && x.User.Id == user.Id);
    }

    public async Task<Models.Map?> UpdateMapAsync(User user, Guid mapId, MapRequestDTO value)
    {
        var savedMap = await GetMapByIdAsync(mapId, user);
        if(savedMap == null)
            return null;

        var updatedMap = _mapper.Map(value, savedMap);

        await _mapRepo.UpdateAsync(updatedMap);

        return updatedMap;
    }

    public async Task<bool> DeleteMapAsync(User user, Guid mapId)
    {
        var savedMap = await GetMapByIdAsync(mapId, user);
        if (savedMap == null)
            return false;

        await _mapRepo.RemoveAsync(savedMap);

        return true;
    }

    public Task<IEnumerable<Models.Map>> GetAllMapsAsync(User user)
    {
        return _mapRepo.GetWhereAsync(x => x.User == user);
    }
}

public static class MapServiceExtensions 
{
    public static IServiceCollection AddMapService(this IServiceCollection services)
    {
        services.AddTransient<IMapService, MapService>();
        return services;
    }
}