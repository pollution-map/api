namespace PollutionMapAPI.DTOs.Entities;

public class MapRefDTO
{
    public string Id { get; set; }

    public string Name { get; set; }

    public DatasetRefDTO Dataset { get; set; }
}
