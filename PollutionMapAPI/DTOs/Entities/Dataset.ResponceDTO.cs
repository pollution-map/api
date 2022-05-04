using PollutionMapAPI.Data.Entities;

namespace PollutionMapAPI.DTOs.Entities;

public class DatasetResponceDTO
{
    public string Id { get; set; }
    public Dictionary<string, DatasetPropertyType> Schema { get; set; } = new Dictionary<string, DatasetPropertyType>();
    public List<DatasetItemResponceDTO> Items { get; set; }
}
