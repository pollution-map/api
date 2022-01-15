namespace PollutionMapAPI.DTOs.Entities;

public class MapResponceDTO : MapRefDTO
{
    // Temporary property. It should be replaced with actual ui elements info in the future.
    public string[] SomeUiElements { get; set; }

    public MapResponceDTO()
    {
        var random = new Random();
        SomeUiElements = new string[] { "Slider", "Dropdown menu", "Toggle", "Switch btns" }
            .Where(item => random.Next(2) == 1).ToArray();
    }
}