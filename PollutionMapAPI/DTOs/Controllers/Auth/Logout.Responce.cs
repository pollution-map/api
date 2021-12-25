namespace PollutionMapAPI.DTOs;

public class LogoutRespone : Responce
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public UserResponceDTO? User { get; set; }
}