using AutoMapper;
using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.Helpers;
using System.Text.Json;

namespace PollutionMapAPI.DTOs.Entities;

public class UIElementAutoMapperProfile : Profile
{
    public UIElementAutoMapperProfile()
    {
        CreateMap<UIElement, UIElementResponceDTO>().ForPath(
            uiElementResponceDTO => uiElementResponceDTO.Id,
            opt => opt.MapFrom(uiElement => uiElement.Id.ToBase62FromGuid())
        );

        CreateMap<string, JsonElement>().ConvertUsing(str => ConvertToJsonElement(str));
    }

    private static JsonElement ConvertToJsonElement(string str)
    {
        using var jsonDocument = JsonDocument.Parse(str);
        return jsonDocument.RootElement.Clone();
    }
}
