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

    [HttpGet("confirm")]

    public async Task<ActionResult<Responce>> ConfirmEmail([FromQuery] string id, [FromQuery] string confirmationToken)
    {
        var result = await _emailConfirmationService.ConfirmEmailAsync(id, confirmationToken);

        if (!result.EmailConfirmed)
            return BadRequest(Responce.FromError("Bad request."));

        return Ok(Responce.FromSuccess("Email confirmation successful."));
    }
    
    [HttpPost("confirm/send")]
    public async Task<ActionResult<Responce>> SendConfirmationEmail([FromQuery] string id)
    {
        var result = await _emailConfirmationService
            .SendConfirmationEmailAsync(id);

        if (!result.UserFound || 
            !result.EmailFound || 
             result.EmailConfirmed || 
            !result.ConfirmationEmailSent)
            return BadRequest(Responce.FromError("Bad request."));

        return Ok(Responce.FromSuccess("Email confirmation message has been successfully sent."));
    }
}
