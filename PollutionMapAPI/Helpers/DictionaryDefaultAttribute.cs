using System.ComponentModel;

namespace PollutionMapAPI.Helpers;

public class DictionaryDefaultAttribute : DefaultValueAttribute
{
    public DictionaryDefaultAttribute(string key, string value, string key1, string value1)
        : base(new Dictionary<string, string>() { { key, value }, { key1, value1 } })
    {
    }
}