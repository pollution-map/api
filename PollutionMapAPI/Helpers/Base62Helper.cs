using Base62;

namespace PollutionMapAPI.Helpers;

public static class Base62HelperExtensions
{
    public static readonly Base62Converter _conveter = new();
    public static string ToBase62FromGuid(this Guid guid)
    {
        return _conveter.Encode(guid.ToString());
    }

    public static Guid? ToGuidFromBase62(this string base62Str)
    {
        try
        {
            return new Guid(_conveter.Decode(base62Str));
        }
        catch
        {
            return null;
        }
    }
}
