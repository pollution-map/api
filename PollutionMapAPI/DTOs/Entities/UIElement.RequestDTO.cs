using PollutionMapAPI.Data.Entities;
using System.Text.Json;

namespace PollutionMapAPI.DTOs.Entities;

public class UIElementRequestDTO
{
    public UIElementType Type { get; set; }

    public HashSet<string> PropertiesNames { get; set; }

    public JsonElement Style { get; set; }
}
