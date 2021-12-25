﻿using PollutionMapAPI.Repositories.Core;

namespace PollutionMapAPI.Models;

public class RefreshToken : BaseEntity
{
    // Randomly generated refresh token value
    public string Token { get; set; }
    
    // Link to a user the token is given to
    public string UserId { get; set; }
    public User User { get; set; }

    // Token has limited lifetime 
    public DateTime HasBeenCreatedOn { get; set; }
    public DateTime WillExpireOn { get; set; }

    // Ip what created this token
    public string Ip { get; set; }
}

public static class RefreshTokenExtensions
{
    public static bool HasExpired(this RefreshToken token)
    {
        return DateTime.UtcNow > token.WillExpireOn;
    }
}