using PollutionMapAPI.DTOs.Entities;

namespace PollutionMapAPI.DTOs;

public class LoginResponce
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public UserResponceDTO? User { get; set; }
}