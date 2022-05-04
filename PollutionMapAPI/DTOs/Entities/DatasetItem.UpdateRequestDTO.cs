namespace PollutionMapAPI.DTOs.Entities;

public class DatasetItemUpdateRequestDTO : DatasetItemRequestDTO
{
    public new double? Latitude { get; set; }

    public new double? Longitude { get; set; }

    public new Dictionary<string, string>? Properties { get; set; }
}
