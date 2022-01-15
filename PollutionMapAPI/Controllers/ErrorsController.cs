using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PollutionMapAPI.Controllers;

[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ControllerBase
{
    [Route("error")]
    public ErrorResponse Error()
    {
        var exception = HttpContext.Features.Get<IExceptionHandlerFeature>().Error;

        Response.StatusCode = exception.StatusCode();

        return new ErrorResponse(exception);
    }
}

public static class StatusCodeEx
{
    public static int StatusCode(this Exception exception) => exception switch
    {
        BadRequest400Exception => 400, // Bad Request
        Unauthorized401Exception => 401, // Unauthorized
        Forbidden403Exception => 403, // Forbidden
        NotFound404Exception => 404, // Not Found
        CouldNotCreate422Exception => 422,
        CouldNotUpdate422Exception => 422,
        CouldNotDelete422Exception => 422,
        _ => 500, // Internal Server Error by default
    };
}

/// <summary>
/// Exception expected by API client
/// </summary>
public abstract class ExpectedException : Exception
{
    public ExpectedException(string? message) : base(message)
    {
    }
    
    public override string StackTrace
    {
        // Expected exceptions should not have stack trace to prevent exposing internal app structure
        get { return ""; }
    }
}
public class BadRequest400Exception : ExpectedException
{
    public BadRequest400Exception(string? message) : base(message) { }
}

public class Unauthorized401Exception : ExpectedException
{
    public Unauthorized401Exception(string? message) : base(message) { }
}

public class Forbidden403Exception : ExpectedException
{
    public Forbidden403Exception(string? message) : base(message) { }
}

public class NotFound404Exception : ExpectedException
{
    public NotFound404Exception(string? message) : base(message) { }
}

public class CouldNotCreate422Exception : ExpectedException
{
    public CouldNotCreate422Exception(string? message) : base(message) { }
}

public class CouldNotUpdate422Exception : ExpectedException
{
    public CouldNotUpdate422Exception(string? message) : base(message) { }
}

public class CouldNotDelete422Exception : ExpectedException
{
    public CouldNotDelete422Exception(string? message) : base(message) { }
}

public class ErrorResponse
{
    public string Type { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }

    public ErrorResponse(Exception ex)
    {
        Type = ex.GetType().Name;
        Message = ex.Message;

        if(ex is ExpectedException)
        {
            StackTrace = "Expected error occurred no stack trace is shown.";
        }
            
        else
            StackTrace = ex.ToString();
    }
}

public static class IdentityErrorExtensions
{
    public static string AsString(this IEnumerable<IdentityError> errors)
    {
        return string.Join("\n", errors.Select(e => $"{e.Description}"));
    }
}