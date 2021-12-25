using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PollutionMapAPI.DTOs;
using PollutionMapAPI.Models;
using PollutionMapAPI.Services.Auth;
using PollutionMapAPI.Services.Email;
using System.Security.Claims;

namespace PollutionMapAPI.Controllers;
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IAuthService _authService;
    private readonly AuthServiceSettings _authServiceSettings;
    private readonly IEmailConfrimationService _emailConfrimationService;
    private readonly IMapper _autoMapper;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IAuthService authService,
        AuthServiceSettings authServiceSettings,
        IEmailConfrimationService emailConfrimationService,
        IMapper autoMapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authService = authService;
        _authServiceSettings = authServiceSettings;
        _emailConfrimationService = emailConfrimationService;
        _autoMapper = autoMapper;
    }


    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponce>> Register([FromBody] RegisterRequest registerRequest)
    {
        var user = new User()
        {
            UserName = registerRequest.UserName,
            Email = registerRequest.Email,
        };

        var userCreationResut = await _userManager.CreateAsync(user);

        if (!userCreationResut.Succeeded)
            return BadRequest(new RegisterResponce().WithIdentityErrors(userCreationResut.Errors));

        var passwordAddingResult = await _userManager.AddPasswordAsync(user, registerRequest.Password);
        if (!passwordAddingResult.Succeeded)
            return BadRequest(new RegisterResponce().WithIdentityErrors(passwordAddingResult.Errors));

        var isEmailProvided = !string.IsNullOrEmpty(user.Email);
        if (isEmailProvided && _userManager.Options.SignIn.RequireConfirmedEmail)
            await _emailConfrimationService.SendConfirmationEmailAsync(user.Id);

        var createdUserInfo = _autoMapper.Map<UserResponceDTO>(user);
        return Created($"api/users/{createdUserInfo.Id}", new RegisterResponce()
        {
            User = createdUserInfo,
        }.WithSuccessMessage("Registration successful."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponce>> Login([FromBody] LoginRequest loginRequest)
    {
        var user = 
               await _userManager.FindByNameAsync(loginRequest.UserName) 
            ?? await _userManager.FindByEmailAsync(loginRequest.Email);
        if (user is null)
            return Unauthorized(LoginResponce.FromError("Invalid credentials"));

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
        
        if(signInResult.IsNotAllowed)
            return Unauthorized(LoginResponce.FromError("Is not allowed to sign in"));

        if (!signInResult.Succeeded)
            return Unauthorized(LoginResponce.FromError("Invalid credentials"));

        var accessToken = _authService.GenerateAccessToken(user);
        var refreshToken = _authService.GenerateRefreshToken();

        await _authService.SaveRefreshTokenAsync(user, refreshToken, GetRequestIp());

        SetRefreshTokenCookie(refreshToken);

        return Ok(new LoginResponce() { 
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = _autoMapper.Map<UserResponceDTO>(user),
        }.WithSuccessMessage("Login successful."));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshResponce>> Refresh([FromBody] RefreshRequest refreshRequest)
    {
        ClaimsPrincipal principal;
        try
        {
            principal = _authService.GetClaimsPrincipalFromExpiredAccessToken(refreshRequest.AccessToken);
        }
        catch (ArgumentException)
        {
            return BadRequest(RefreshResponce.FromError("Invalid access token format"));
        }
        catch (Exception)
        {
            return BadRequest(RefreshResponce.FromError("Invalid access token"));
        }

        var refreshToken = refreshRequest.RefreshToken;

        var user = await _userManager.FindByNameAsync(principal?.Identity?.Name ?? "");

        if (!await _authService.IsRefreshTokenValid(user, refreshToken))
            return BadRequest(RefreshResponce.FromError("Invalid refresh token"));

        var newAccessToken  = _authService.GenerateAccessToken(user);
        var newRefreshToken = _authService.GenerateRefreshToken();

        await _authService.DeleteRefreshTokenAsync(user, refreshToken);
        await _authService.SaveRefreshTokenAsync(user, newRefreshToken, GetRequestIp());

        SetRefreshTokenCookie(newRefreshToken);

        return Ok(new RefreshResponce() { 
            AccessToken = newAccessToken, 
            RefreshToken = newRefreshToken 
        }.WithSuccessMessage("Refresh successful."));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<LogoutRespone>> Logout([FromBody] LogoutRequest logoutRequest)
    {
        var refreshToken = logoutRequest.RefreshToken ?? GetRefreshTokenFromCookie();

        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest(LogoutRespone.FromError("Token is required."));

        var user = await _userManager.FindByNameAsync(this.User!.Identity!.Name);

        await _authService.DeleteRefreshTokenAsync(user, refreshToken);

        if (logoutRequest.FromAllDevices)
            await _authService.DeleteAllRefreshTokensAsync(user);

        SetRefreshTokenCookie("");

        return Ok(LogoutRespone.FromSuccess("Logout successfull."));
    }

    private string? GetRefreshTokenFromCookie()
    {
        return Request.Cookies["refreshToken"];
    }
    
    private void SetRefreshTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(_authServiceSettings.RefreshTokenLivesInDays)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    private string GetRequestIp()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];
        else
            return HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
    }
}