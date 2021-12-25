namespace PollutionMapAPI.Services.Auth;

public class AuthServiceSettings
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int RefreshTokenLivesInDays { get; set; }
    public int AccessTokenLivesInSeconds { get; set; }
    public bool RequireConfirmedEmailToLogin { get; set; }
}