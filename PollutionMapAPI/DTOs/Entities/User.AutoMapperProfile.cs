using AutoMapper;

namespace PollutionMapAPI.DTOs.Entities;

public class UserAutoMapperProfile : Profile
{
    public UserAutoMapperProfile()
    {
        CreateMap<Models.User, UserResponceDTO>().ForPath(
            userResponceDTO => userResponceDTO.Id, 
            opt => opt.MapFrom(
                user => user.Id.ToString()
            )
        );
    }
}
