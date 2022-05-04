namespace PollutionMapAPI.DTOs.Entities;

public class DatasetItemRequestDTO
{
    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public Dictionary<string, string> Properties { get; set; }
}
