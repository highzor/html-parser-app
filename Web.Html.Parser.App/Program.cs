using Microsoft.Extensions.Configuration;
using Web.Html.Parser.App;
using Web.Html.Parser.App.Data;
using Web.Html.Parser.App.Domain;
using Web.Html.Parser.App.Models.Configuration;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json", optional: false);

        var config = builder.Build();

        var appSettings = config.GetSection("ApplicationConfiguration").Get<ApplicationConfiguration>();
        var connectionStrings = config.GetSection("ConnectionStrings").Get<ConnectionStrings>();
        connectionStrings.PostgresDatabase = connectionStrings.PostgresDatabase.Replace("[password]", Environment.GetEnvironmentVariable("postgres"));

        new ParserApplication(new ParserManager(new ParserRepository(connectionStrings.PostgresDatabase))).Start(appSettings.Url).GetAwaiter().GetResult();
    }
}