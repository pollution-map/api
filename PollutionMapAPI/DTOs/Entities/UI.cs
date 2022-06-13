using AutoMapper;
using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.Helpers;

namespace PollutionMapAPI.DTOs.Entities;

public class MapUIAutoMapperProfile : Profile
{
    public MapUIAutoMapperProfile()
    {
        CreateMap<UI, UIRefDTO>().ForPath(
            uiRefDTO => uiRefDTO.Id,
            opt => opt.MapFrom(
                ui => ui.Id.ToBase62FromGuid()
            )
        );

        CreateMap<UI, UIResponceDTO>().ForPath(
            uiRefDTO => uiRefDTO.Id,
            opt => opt.MapFrom(
                ui => ui.Id.ToBase62FromGuid()
            )
        );
    }
}
