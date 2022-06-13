using AutoMapper;
using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.DTOs.Entities;
using PollutionMapAPI.Repositories;
using PollutionMapAPI.Services.Dataset;

namespace PollutionMapAPI.Services.Map;

public class MapService : IMapService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MapService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Data.Entities.Map?> CreateMapAsync(Guid userId, MapRequestDTO value)
    {
        var map = _mapper.Map<Data.Entities.Map>(value);
        map.UserId = userId;

        // Create empty dataset according to the schema
        var dataset = new Data.Entities.Dataset() { Map = map };
        var schema = DatasetSchema.Empty;
        var properties = schema.ToList();
        foreach (var property in properties)
        {
            property.DataSet = dataset;
            property.DataSetId = dataset.Id;
        }
        dataset.Properties = properties;
        dataset.Items = new List<DatasetItem>();

        map.Dataset = dataset;
        map.DatasetId = dataset.Id;

        // Create new map ui
        var mapUi = new Data.Entities.UI() { 
            Map = map, 
            Elements = new() 
        };

        map.UI = mapUi;

        _unitOfWork.UIRepository.Add(mapUi);
        _unitOfWork.DatasetRepository.Add(dataset);
        _unitOfWork.MapRepository.Add(map);
        await _unitOfWork.SaveChangesAsync();

        return map;
    }

    public Task<Data.Entities.Map?> GetMapByIdAsync(Guid mapId)
    {
        return _unitOfWork.MapRepository.FirstOrDefaultAsync(x => x.Id == mapId);
    }

    private Task<Data.Entities.Map?> GetMapByIdAsync(Guid mapId, Guid userId)
    {
        return _unitOfWork.MapRepository.FirstOrDefaultAsync(x => x.Id == mapId && x.User.Id == userId);
    }

    public async Task<Data.Entities.Map?> UpdateMapAsync(Guid userId, Guid mapId, MapRequestDTO value)
    {
        var savedMap = await GetMapByIdAsync(mapId, userId);
        if(savedMap == null)
            return null;

        var updatedMap = _mapper.Map(value, savedMap);

        _unitOfWork.MapRepository.Update(updatedMap);
        await _unitOfWork.SaveChangesAsync();

        return updatedMap;
    }

    public async Task<bool> DeleteMapAsync(Guid userId, Guid mapId)
    {
        var savedMap = await GetMapByIdAsync(mapId, userId);
        if (savedMap == null)
            return false;

        _unitOfWork.DatasetRepository.Remove(savedMap.Dataset);
        _unitOfWork.UIRepository.Remove(savedMap.UI);
        _unitOfWork.MapRepository.Remove(savedMap);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public Task<IEnumerable<Data.Entities.Map>> GetAllMapsAsync(Guid userId)
    {
        return _unitOfWork.MapRepository.GetWhereAsync(x => x.UserId == userId);
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