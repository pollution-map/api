using System.ComponentModel;

namespace PollutionMapAPI.DTOs;
public class UserResponceDTO
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    
    [DefaultValue(false)]
    public bool EmaiConfirmed { get; set; }
}