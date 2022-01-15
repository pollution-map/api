using System.ComponentModel.DataAnnotations;

namespace PollutionMapAPI.DTOs;

public class LoginRequest
{
    [Required]
    public string Login { get; set; }

    [Required]
    public string Password { get; set; }
}
