using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PollutionMapAPI.DTOs;
using PollutionMapAPI.DTOs.Entities;
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

    /// <summary> Register a new user </summary>
    /// <remarks> Registeres a new user and automaticly sends confirmation email if needed. </remarks>
    /// <response code="201">Registration successful.</response>
    /// <response code="400">User could not be created. May be a dublicate Email or Username.</response>
    /// <param name="registerRequest">User credentials. Email and Username should be unique.</param>

    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResponceDTO))]
    [HttpPost("register")]
    public async Task<ActionResult<UserResponceDTO>> Register([FromBody] RegisterRequest registerRequest)
    {
        var user = new User()
        {
            UserName = registerRequest.Username,
            Email = registerRequest.Email,
        };

        var userCreationResut = await _userManager.CreateAsync(user);

        if (!userCreationResut.Succeeded)
            throw new BadRequest400Exception(userCreationResut.Errors.AsString());

        var passwordAddingResult = await _userManager.AddPasswordAsync(user, registerRequest.Password);
        if (!passwordAddingResult.Succeeded)
            throw new BadRequest400Exception(passwordAddingResult.Errors.AsString());

        var isEmailProvided = !string.IsNullOrEmpty(user.Email);
        if (isEmailProvided && _userManager.Options.SignIn.RequireConfirmedEmail && _authServiceSettings.RequireConfirmedEmailToLogin)
            await _emailConfrimationService.SendConfirmationEmailAsync(user.Id.ToString());

        var createdUserInfo = _autoMapper.Map<UserResponceDTO>(user);
        return Created($"api/users/{createdUserInfo.Id}", createdUserInfo);
    }

    /// <summary> Login to get Access_Token and Refresh_Token</summary>
    /// <remarks>
    /// Generate Refresh-Access token pair for JWT Bearer Authentication. 
    /// You may want to confirm email before login. 
    /// <br/>
    /// You should avoid using this end-point for getting new Access_Tokens if you allready got one.
    /// <br/>
    /// Each Login should be perfromed successfuly once per user device session and end up with Logout or Refresh_Token expiration.
    /// <br/>
    /// Usage of login should look like this
    /// 
    ///     PC: Login ---> ....refresh... ...refresh... ---> Logout
    ///     Mobile: Login ---> ....refresh... ...refresh... ...refresh... ---> Logout
    /// 
    /// NOT like this
    /// 
    ///     PC: Login ---> Login ---> Login ---> Login
    /// 
    /// If you need to get new Access_Token use your Refresh_Token to refresh it using /refesh endpoint.
    /// </remarks>
    /// <param name="loginRequest">Login and Password. Login is Username or Email.</param>
    /// <response code="200">Successfully logged in</response>
    /// <response code="403"> Could not authorize. Invalid user credentials or unconfirmed email.</response>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponce>> Login([FromBody] LoginRequest loginRequest)
    {
        var user = 
               await _userManager.FindByNameAsync(loginRequest.Login) 
            ?? await _userManager.FindByEmailAsync(loginRequest.Login);
        if (user is null)
            throw new Forbidden403Exception("Invalid credentials");

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
        
        if(signInResult.IsNotAllowed)
            throw new Forbidden403Exception("Is not allowed to sign in");

        if (!signInResult.Succeeded)
            throw new Forbidden403Exception("Invalid credentials");

        var accessToken = _authService.GenerateAccessToken(user);
        var refreshToken = _authService.GenerateRefreshToken();

        await _authService.SaveRefreshTokenAsync(user, refreshToken, GetRequestIp());

        return Ok(new LoginResponce() { 
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = _autoMapper.Map<UserResponceDTO>(user),
        });
    }


    /// <summary> Refresh Access_Token with Refresh_Token </summary>
    /// <response code="200">Successfully refreshed access token with refresh token</response>
    /// <response code="400">Access/Refresh token is invalid</response>
    /// <returns>Refresh-Access token pair</returns>

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
            throw new BadRequest400Exception("Invalid access token format");
        }
        catch (Exception)
        {
            throw new BadRequest400Exception("Invalid access token");
        }

        var refreshToken = refreshRequest.RefreshToken;

        var user = await _userManager.FindByNameAsync(principal?.Identity?.Name ?? "");

        if (!await _authService.IsRefreshTokenValid(user, refreshToken))
            throw new BadRequest400Exception("Invalid refresh token");

        var newAccessToken  = _authService.GenerateAccessToken(user);
        var newRefreshToken = _authService.GenerateRefreshToken();

        await _authService.DeleteRefreshTokenAsync(user, refreshToken);
        await _authService.SaveRefreshTokenAsync(user, newRefreshToken, GetRequestIp());

        return Ok(new RefreshResponce() {
            AccessToken = newAccessToken, 
            RefreshToken = newRefreshToken 
        });
    }

    /// <summary> Log out to invalidate Refresh_Token </summary>
    /// <remarks>Invalidates Refresh-Access token pair</remarks>
    /// <response code="200">Successfully logged out</response>
    /// <response code="400">Refresh token is not provided</response>

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<Responce>> Logout([FromBody] LogoutRequest logoutRequest)
    {
        var refreshToken = logoutRequest.RefreshToken;

        if (string.IsNullOrEmpty(refreshToken))
            throw new BadRequest400Exception("Token is required.");

        var user = await _userManager.FindByNameAsync(this.User!.Identity!.Name);

        await _signInManager.SignOutAsync();
        await _authService.DeleteRefreshTokenAsync(user, refreshToken);

        if (logoutRequest.FromAllDevices)
            await _authService.DeleteAllRefreshTokensAsync(user);

        return Ok(Responce.FromSuccess("Successfully logged out."));
    }

    private string GetRequestIp()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];
        else
            return HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
    }
}