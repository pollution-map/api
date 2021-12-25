using AutoMapper;

namespace PollutionMapAPI.DTOs;

public class UserAutoMapperProfile : Profile
{
    public UserAutoMapperProfile()
    {
        CreateMap<Models.User, UserResponceDTO>();
    }
}
