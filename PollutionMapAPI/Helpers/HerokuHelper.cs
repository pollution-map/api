using Npgsql;

namespace PollutionMapAPI.Helpers;

public static class HerokuHelper
{
    public static string? GetPostgersConnectionString()
    {
        var herokuEnvVarPostgressDbConnectionStr =  Environment.GetEnvironmentVariable("DATABASE_URL");

        if (string.IsNullOrEmpty(herokuEnvVarPostgressDbConnectionStr))
            return null;

        var databaseUri = new Uri(herokuEnvVarPostgressDbConnectionStr);
        var userInfo = databaseUri.UserInfo.Split(':');
        var connectionBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.LocalPath.TrimStart('/'),
            SslMode = SslMode.Require,
            TrustServerCertificate = true,
        };
        var str = connectionBuilder.ToString();
        return str;
    }
}
