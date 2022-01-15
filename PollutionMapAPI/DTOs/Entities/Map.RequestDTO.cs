using System.ComponentModel;

namespace PollutionMapAPI.DTOs.Entities;

public class MapRequestDTO
{
    [DefaultValue("Untitled")]
    public string Name { get; set; } = "Untitled";
}
