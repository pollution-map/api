using System.ComponentModel.DataAnnotations;

namespace PollutionMapAPI.DTOs;

public class UserRequestDTO
{
    public string UserName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
