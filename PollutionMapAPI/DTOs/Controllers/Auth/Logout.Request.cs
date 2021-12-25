using System.ComponentModel;

namespace PollutionMapAPI.DTOs;

public class LogoutRequest
{
    public string RefreshToken { get; set; }

    [DefaultValue(false)]
    public bool FromAllDevices { get; set; }
}
