namespace PollutionMapAPI.DTOs;

public class LoginResponce : Responce
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public UserResponceDTO? User { get; set; }
}