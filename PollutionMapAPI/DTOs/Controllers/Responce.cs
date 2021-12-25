using Microsoft.AspNetCore.Identity;

namespace PollutionMapAPI.DTOs;

public class Responce
{
    public string? Message { get; set; } = null;
    public bool? Success { get; set; } = null;

    public static Responce FromSuccess(string message)
    {
        return new Responce() { Message = message, Success = true };
    }

    public static Responce FromError(string message)
    {
        return new Responce() { Message = message, Success = false };
    }
}

public static class ResponceWithMessageExtensions
{
    public static Responce WithErrorMessage(this Responce responceDto, string message)
    {
        if (string.IsNullOrEmpty(message)) return responceDto;

        responceDto.Message = message;
        responceDto.Success = false;
        return responceDto;
    }
    public static Responce WithSuccessMessage(this Responce responceDto, string message)
    {
        if (string.IsNullOrEmpty(message)) return responceDto;

        responceDto.Message = message;
        responceDto.Success = true;
        return responceDto;
    }
}

public static class ResponceWithMessageIdentityExtensions
{
    public static Responce WithIdentityErrors(this Responce responceDto, IEnumerable<IdentityError> errors)
    {
        var multiLineErrorMessage = string.Join("\n", errors.Select(e => $"{e.Description}"));
        responceDto.WithErrorMessage(multiLineErrorMessage);
        responceDto.Success = string.IsNullOrEmpty(multiLineErrorMessage);
        return responceDto;
    }
}