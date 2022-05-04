namespace PollutionMapAPI.DTOs.Entities;

public class MapResponceDTO
{
    public string Id { get; set; }
    public string Name { get; set; }

    // Temporary property. It should be replaced with actual ui elements info in the future.
    public string[] SomeUiElements { get; set; }
    public DatasetResponceDTO Dataset { get; set; }

    public MapResponceDTO()
    {
        var random = new Random();
        SomeUiElements = new string[] { "Slider", "Dropdown menu", "Toggle", "Switch btns" }
            .Where(item => random.Next(2) == 1).ToArray();
    }
}