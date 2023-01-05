using System.Globalization;
using Web.Html.Parser.App.Domain;
using Web.Html.Parser.App;
using Microsoft.Extensions.Configuration;
using Web.Html.Parser.App.Models.Configuration;

namespace Web.Html.Parser.App;

public class ParserApplication
{
    private readonly IParserManager _parserManager;

    public ParserApplication(IParserManager parserManager)
    {
        _parserManager = parserManager;
    }
    public async Task Start()
    {
        var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false);

        IConfiguration config = builder.Build();

        var appSettings = config.GetSection("ApplicationConfiguration").Get<ApplicationConfiguration>();

        var document = await _parserManager.GetDocument(appSettings.Url);
        var games = document.All.Where(letter => letter.GetAttribute("class") == "promo-game-item").ToList();

        foreach (var game in games)
        {
            var gameDivContainer = game.Children.Where(child => child.GetAttribute("class") == "game-title").FirstOrDefault();
            var gameName = gameDivContainer?.Children.Where(child => child.LocalName == "a").FirstOrDefault()?.TextContent.Trim();
            int.TryParse(gameDivContainer?.GetAttribute("data-id"), NumberStyles.Number, CultureInfo.InvariantCulture, out int gameWebId);

            //_parserManager.AddUpdateGame(gameName, gameWebId);
            //return: gameId  

            var gameItems = game.Children.Where(child => child.LocalName == "ul").FirstOrDefault()?.Children.Where(child => child.LocalName == "li").ToList();

            foreach (var gameItem in gameItems)
            {
                var gameItemLinkElement = gameItem.Children.Where(child => child.LocalName == "a").FirstOrDefault();
                var gameItemHref = gameItemLinkElement?.GetAttribute("href");
                var gameItemTitle = gameItemLinkElement?.TextContent.Trim();

                //_parserManager.AddUpdateGameItem(gameId, gameItemTitle);
                //return: void
            }

            var stop = "";
        }
    }
}