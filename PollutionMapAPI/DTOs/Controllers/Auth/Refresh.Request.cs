using System.ComponentModel.DataAnnotations;

namespace PollutionMapAPI.DTOs;

public class RefreshRequest
{
    [Required]
    public string AccessToken { get; set; }
    
    [Required]
    public string RefreshToken { get; set; }
}
