using System.ComponentModel.DataAnnotations;

namespace PollutionMapAPI.DTOs;

public class RegisterRequest
{
    public string Username { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
