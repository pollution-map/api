using System.ComponentModel;
using System.Reflection;

namespace PollutionMapAPI.Helpers;

public static class EnumExtensions
{
    public static string? ToDescriptionString<TEnum>(this TEnum @enum) where TEnum : Enum
    {
        FieldInfo info = @enum.GetType().GetField(@enum.ToString());
        var attributes = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);

        return attributes?[0].Description ?? @enum.ToString();
    }
}
