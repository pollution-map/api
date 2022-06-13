using PollutionMapAPI.Data.Entities;
using System.Text.Json;

namespace PollutionMapAPI.DTOs.Entities;

public class UIElementResponceDTO
{
    public string Id { get; set; }

    public UIElementType Type { get; set; }

    public Dictionary<string, DatasetPropertyType> Properties { get; set; }

    public JsonElement Style { get; set; } 
}
