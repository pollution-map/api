using System.ComponentModel;

namespace PollutionMapAPI.DTOs.Entities;
public class UserResponceDTO
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    
    [DefaultValue(false)]
    public bool EmaiConfirmed { get; set; }

    public IEnumerable<MapRefDTO> Maps { get; set; }
}