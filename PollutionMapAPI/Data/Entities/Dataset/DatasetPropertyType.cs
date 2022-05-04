using System.ComponentModel;

namespace PollutionMapAPI.Data.Entities;

public enum DatasetPropertyType
{
    [Description("Unknown")]
    Unknown = 0,

    [Description("Number")]
    Number = 1,

    [Description("Category")]
    Category = 2,

    [Description("DateTime")]
    DateTime = 3,

    [Description("Bool")]
    Bool = 4
}