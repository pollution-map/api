using System.ComponentModel.DataAnnotations;

namespace PollutionMapAPI.DTOs.Entities;

public class UserRequestDTO
{
    public string? Username { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string Password { get; set; }
} 
