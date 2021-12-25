namespace PollutionMapAPI.DTOs;

public class RefreshResponce : Responce
{
    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }
}
