namespace PollutionMapAPI.Services.Email;

public interface IEmailConfrimationService
{
    Task<EmailConfrimationResult> ConfirmEmailAsync(string userId, string confirmationToken);
    Task<EmailConfrimationSendResult> SendConfirmationEmailAsync(string userId);
}

public class EmailConfrimationResult
{
    public bool UserFound { get; set; }

    public bool EmailConfirmed { get; set; }
}

public class EmailConfrimationSendResult
{
    public bool UserFound { get; set; }

    public bool EmailFound { get; set; }

    public bool EmailConfirmed { get; set; }

    public bool ConfirmationEmailSent { get; set; }
}