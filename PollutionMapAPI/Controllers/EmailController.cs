using Microsoft.AspNetCore.Mvc;
using PollutionMapAPI.DTOs;
using PollutionMapAPI.Services.Email;

namespace PollutionMapAPI.Controllers;
[Route("api/email")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly IEmailConfrimationService _emailConfirmationService;

    public EmailController(IEmailConfrimationService emailConfirmationService)
    {
        _emailConfirmationService = emailConfirmationService;
    }

    /// <summary>
    /// Confirm user's email with EmailConfirmationToken
    /// </summary>
    /// <param name="id">User's id upon whom email confirmation is performed</param>
    /// <param name="confirmationToken">EmailConfirmationToken for User's email confirmation. Generated and sent by api/email/confirm/send endopoint</param>
    /// <remarks>You can recive the email confirmation token from email message sent by api/email/confirm/send endpoint</remarks>
    /// <response code="200">Successfully confirmed the email</response>
    /// <response code="400">Email could not be confirmed. May be invalid user id or confirmation token.</response>

    [HttpGet("confirm")]
    public async Task<ActionResult<Responce>> ConfirmEmail([FromQuery] string id, [FromQuery] string confirmationToken)
    {
        var result = await _emailConfirmationService.ConfirmEmailAsync(id, confirmationToken);

        if (!result.EmailConfirmed)
            throw new BadRequest400Exception("Email could not be confirmed.");

        return Ok(Responce.FromSuccess("Email confirmation successful."));
    }

    /// <summary>
    /// Generate and send email message with EmailConfirmationToken to confirm user's email
    /// </summary>
    /// <param name="id">User's id to send email confirmation message to</param>
    /// <remarks>Recived through email, confirmation token should be GETed with user's id to the api/email/confirm to confrim user's email</remarks>
    /// <response code="200">Successfully confirmed the email</response>
    /// <response code="400">Failed to send confirmation message. Email may be already confirmed, User may not exist or User may not have email provided.</response>
    [HttpPost("confirm/send")]
    public async Task<ActionResult<Responce>> SendConfirmationEmail([FromQuery] string id)
    {
        var result = await _emailConfirmationService
            .SendConfirmationEmailAsync(id);

        if (!result.UserFound || 
            !result.EmailFound || 
             result.EmailConfirmed || 
            !result.ConfirmationEmailSent)
            throw new BadRequest400Exception("Failed to send confirmation message.");

        return Ok(Responce.FromSuccess("Email confirmation message has been successfully sent."));
    }
}
