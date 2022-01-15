using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PollutionMapAPI.Controllers;
using PollutionMapAPI.Models;
using PollutionMapAPI.Repositories.Core;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PollutionMapAPI.Services.Auth;
public class AuthService : IAuthService
{
    private readonly AuthServiceSettings _settings;
    private readonly IAsyncRepository<RefreshToken, long> _refreshTokenRepo;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public AuthService(
        IConfiguration config,
        IAsyncRepository<RefreshToken, long> repository, 
        TokenValidationParameters tokenValidationParameters
    )
    {
        _settings = config.GetRequiredSection("Auth").Get<AuthServiceSettings>();
        _refreshTokenRepo = repository;
        _tokenValidationParameters = tokenValidationParameters;
    }

    public string GenerateAccessToken(User user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                }
            ),
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(_settings.AccessTokenLivesInSeconds),
            SigningCredentials = new SigningCredentials(
                MakeSecretKey(_settings),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetClaimsPrincipalFromExpiredAccessToken(string accessToken)
    {
        // access_token probably expired at this point so no need to validate it's lifetime
        // even if it has not, we don't need to validate it's lifetime anyway
        _tokenValidationParameters.ValidateLifetime = false;

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken)
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    public Task SaveRefreshTokenAsync(User user, string refreshToken, string ip)
    {
        return _refreshTokenRepo.AddAsync(new RefreshToken()
        {
            Token = refreshToken,
            UserId = user.Id.ToString(),
            User = user,
            HasBeenCreatedOn = DateTime.UtcNow,
            WillExpireOn = DateTime.UtcNow.AddDays(_settings.RefreshTokenLivesInDays),
            Ip = ip,
        });
    }

    public async Task<string?> GetRefreshTokenAsync(User user)
    {
        var tokenInfo = await _refreshTokenRepo.FirstOrDefaultAsync(token => 
            token.User == user
        );
        return tokenInfo?.Token;
    }

    public async Task DeleteRefreshTokenAsync(User user, string refreshToken)
    {
        var tokenInfo = await _refreshTokenRepo.FirstOrDefaultAsync(token => token.User == user && token.Token == refreshToken);
        if(tokenInfo is not null)
            await _refreshTokenRepo.RemoveAsync(tokenInfo);
    }

    public async Task DeleteAllRefreshTokensAsync(User user)
    {
        var tokensInfo = await _refreshTokenRepo.GetWhereAsync(token => token.User == user);
        var removeTokens = tokensInfo.Select(token => _refreshTokenRepo.RemoveAsync(token));
        await Task.WhenAll(removeTokens);
    }

    public static SymmetricSecurityKey MakeSecretKey(AuthServiceSettings settings)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
    }

    public async Task<bool> IsRefreshTokenValid(User user, string refreshToken)
    {
        var storedRefreshTokenInfo = await _refreshTokenRepo.FirstOrDefaultAsync(token =>
            token.User == user
            && token.Token == refreshToken
        );

        if (storedRefreshTokenInfo is null) return false;
        if (storedRefreshTokenInfo.HasExpired()) return false;

        return true;
    }
}

public static class AuthExtensions
{
    public static IServiceCollection AddJwtBearerAuth(this IServiceCollection services)
    {
        var settings = services.BuildServiceProvider()
            .GetService<IConfiguration>()
           !.GetSection("Auth")
            .Get<AuthServiceSettings>();

        if (settings.Issuer.IsNullOrEmpty() || settings.Secret.IsNullOrEmpty() || settings.Audience.IsNullOrEmpty())
            throw new Exception("Auth settings are required in appsettings");

        services.AddTransient<AuthServiceSettings>((_) => settings);

        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = settings.Issuer,
            ValidateAudience = true,
            ValidAudience = settings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = AuthService.MakeSecretKey(settings),
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // To immediately reject the access token
        };

        services.AddTransient((_) => tokenValidationParameters);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
           {
               options.SaveToken = true;
               options.TokenValidationParameters = tokenValidationParameters;
               options.Events = new JwtBearerEvents
               {
                   // async is important here 
                   // without async, exception throw crashes the app
                   // with async, exception throw gets handled as error message by /error contoller
                   OnChallenge = async context =>
                   {
                       throw new Unauthorized401Exception("Invalid Bearer access_token or no Bearer access_token provided.");
                   }
               };
           });

        return services.AddScoped<IAuthService, AuthService>();
    }
}

public static class AuthSwaggerExtensions
{
    public static SwaggerGenOptions AddJwtBearerAuthSwaggerUI(this SwaggerGenOptions options)
    {
        // Filter to display a lock icon only on methods where authorization is required
        options.OperationFilter<AuthorizationOperationFilter>();

        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Scheme = "bearer",
            Description = "Please insert JWT token into field"
        });
        return options;
    }
}

// Filter to display a lock icon in swagger docs only on methods where authorization is required
public class AuthorizationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Get the Authorize attribute
        var attributes = context.MethodInfo.DeclaringType!.GetCustomAttributes(true)
                                .Union(context.MethodInfo.GetCustomAttributes(true))
                                .OfType<AuthorizeAttribute>();

        var allowAnonymous = context.MethodInfo.DeclaringType!.GetCustomAttributes(true)
                                .Union(context.MethodInfo.GetCustomAttributes(true))
                                .OfType<AllowAnonymousAttribute>();

        if (attributes.Any() &&!allowAnonymous.Any())
        {
            var attr = attributes.ToList()[0];


            // Add what should be show inside the security section
            var securityInfos = new List<string>
            {
                $"{nameof(AuthorizeAttribute.Policy)}:{attr.Policy}",
                $"{nameof(AuthorizeAttribute.Roles)}:{attr.Roles}",
                $"{nameof(AuthorizeAttribute.AuthenticationSchemes)}:{attr.AuthenticationSchemes}"
            };
            operation.Security = new List<OpenApiSecurityRequirement>()
            {
                new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = JwtBearerDefaults.AuthenticationScheme,
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        securityInfos
                    }
                }
            };
        }
    }
}