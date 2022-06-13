namespace PollutionMapAPI.DTOs.Entities;

public class MapResponceDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public UIResponceDTO UI { get; set; }
    public DatasetResponceDTO Dataset { get; set; }
}