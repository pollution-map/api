using AutoMapper;
using PollutionMapAPI.Helpers;

namespace PollutionMapAPI.DTOs.Entities;

public class MapAutoMapperProfile : Profile
{
    public MapAutoMapperProfile()
    {
        CreateMap<MapRequestDTO, Data.Entities.Map>();
        CreateMap<Data.Entities.Map, MapResponceDTO>().ForPath(
            mapResponceDto => mapResponceDto.Id,
            opt => opt.MapFrom(
                map => map.Id.ToBase62FromGuid()
            )
        );
        CreateMap<Data.Entities.Map, MapRefDTO>().ForPath(
            mapRefDto => mapRefDto.Id,
            opt => opt.MapFrom(
                map => map.Id.ToBase62FromGuid()
            )
        );
    }
}