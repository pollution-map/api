namespace PollutionMapAPI.DTOs.Entities;

public class DatasetItemResponceDTO
{
    public string Id { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public Dictionary<string, string> Properties { get; set; }
}
