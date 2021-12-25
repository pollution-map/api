using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Core;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using PollutionMapAPI.Models;

namespace PollutionMapAPI.Services.Email;

public class MailKitEmailConfirmationService : IEmailConfrimationService
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly EmailConfirmationSettings _settings;

    public MailKitEmailConfirmationService(
        UserManager<User> userManager, 
        IEmailService emailService, 
        IConfiguration config)
    {
        _userManager = userManager;
        _emailService = emailService;
        _settings = config.GetRequiredSection("EmailConfirmation").Get<EmailConfirmationSettings>();
    }
    public async Task<EmailConfrimationResult> ConfirmEmailAsync(string userId, string confirmationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new EmailConfrimationResult()
            {
                UserFound = false,
                EmailConfirmed = false,
            };

        var confirmationResult = await _userManager.ConfirmEmailAsync(user, confirmationToken);

        if (!confirmationResult.Succeeded)
            return new EmailConfrimationResult()
            {
                UserFound = true,
                EmailConfirmed = false,
            };

        return new EmailConfrimationResult()
        {
            UserFound = true,
            EmailConfirmed = true,
        };
    }

    public async Task<EmailConfrimationSendResult> SendConfirmationEmailAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new EmailConfrimationSendResult()
            {
                UserFound = false,
                EmailFound = false,
                EmailConfirmed = false,
                ConfirmationEmailSent = false,
            };

        if (user.Email.IsNullOrEmpty())
            return new EmailConfrimationSendResult() {
                UserFound = true,
                EmailFound = false,
                EmailConfirmed = false,
                ConfirmationEmailSent = false
            };

        if (user.EmailConfirmed)
            return new EmailConfrimationSendResult()
            {
                UserFound = true,
                EmailFound = true,
                EmailConfirmed = true,
                ConfirmationEmailSent = false,
            };

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var emailConfirmationQuery = new QueryBuilder
        {
            { "id", user.Id },
            { "confirmationToken", token }
        }.ToString();

        var emailConfirmationLink = _settings.EmailConfirmationHadlerUrl + emailConfirmationQuery;

        await _emailService.SendAsync(user.Email, "Email confirmation", $"<h1><a href='{emailConfirmationLink}'>Confirm Email</a><h1/><br/><h2>Email confirmation token:</h2><h3>{token}</h3>", true);

        return new EmailConfrimationSendResult()
        {
            UserFound = true,
            EmailFound = true,
            EmailConfirmed = false,
            ConfirmationEmailSent = true
        };
    }
}

public static class MailKitEmailConfirmationServiceExtensions
{
    public static IServiceCollection AddMailKitEmailConfirmation(this IServiceCollection services)
    {
        var mailKitConfig = services.BuildServiceProvider()
            .GetService<IConfiguration>()
           !.GetRequiredSection("Email")
            .Get<MailKitOptions>();

        services.AddMailKit(opt => opt.UseMailKit(mailKitConfig));

        services.AddTransient<IEmailConfrimationService, MailKitEmailConfirmationService>();

        return services;
    }
}
