using AutoMapper;
using PollutionMapAPI.Helpers;

namespace PollutionMapAPI.DTOs.Entities;

public class MapAutoMapperProfile : Profile
{
    public MapAutoMapperProfile()
    {
        CreateMap<MapRequestDTO, Models.Map>();
        CreateMap<Models.Map, MapResponceDTO>().ForPath(
            mapResponceDto => mapResponceDto.Id,
            opt => opt.MapFrom(
                map => map.Id.ToBase62FromGuid()
            )
        );
        CreateMap<Models.Map, MapRefDTO>().ForPath(
            mapRefDto => mapRefDto.Id,
            opt => opt.MapFrom(
                map => map.Id.ToBase62FromGuid()
            )
        );
    }
}