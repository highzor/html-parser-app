using System.Globalization;
using Web.Html.Parser.App.Domain;
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
        var games = document
                    .All
                    .Where(letter => letter.GetAttribute("class") == "promo-game-item")
                    .ToList();

        foreach (var game in games)
        {
            var gameDivContainer = game
                                   .Children
                                   .Where(child => child.GetAttribute("class") == "game-title")
                                   .FirstOrDefault();

            var gameName = gameDivContainer
                           ?.Children
                           .Where(child => child.LocalName == "a")
                           .FirstOrDefault()
                           ?.TextContent
                           .Trim();

            int.TryParse(gameDivContainer?.GetAttribute("data-id"), NumberStyles.Number, CultureInfo.InvariantCulture, out int gameWebId);

            //_parserManager.AddUpdateGame(gameWebId, gameName);
            //return: gameId  

            var gameItems = game
                            .Children
                            .Where(child => child.LocalName == "ul")
                            .FirstOrDefault()
                            ?.Children
                            .Where(child => child.LocalName == "li")
                            .ToList();

            foreach (var gameItem in gameItems)
            {
                var gameItemLinkElement = gameItem
                                          .Children
                                          .Where(child => child.LocalName == "a")
                                          .FirstOrDefault();

                var gameItemHref = gameItemLinkElement?.GetAttribute("href");
                var gameItemTitle = gameItemLinkElement
                                    ?.TextContent
                                    .Trim();

                //_parserManager.AddUpdateGameItem(gameId, gameItemTitle);
                //return: void

                document = await _parserManager.GetDocument(gameItemHref);
                var items = document.All.Where(letter => letter.GetAttribute("class") == "tc-item").ToList();

                foreach (var item in items)
                {
                    var userUrl = item.GetAttribute("href");
                    var userWebId = userUrl.Substring(userUrl.IndexOf('=') + 1);
                    var userName = item
                                   ?.Children
                                   .Where(child => child.GetAttribute("class") == "tc-user")
                                   .FirstOrDefault()
                                   ?.Children
                                   .Where(child => child.GetAttribute("class").Contains("media-user"))
                                   .FirstOrDefault()
                                   ?.Children
                                   .Where(child => child.GetAttribute("class") == "media-body")
                                   .FirstOrDefault()
                                   ?.Children
                                   .Where(child => child.GetAttribute("class") == "media-user-name")
                                   .FirstOrDefault()
                                   .TextContent
                                   ?.Trim();

                    //_parserManager.AddUpdateUser(userWebId, userName);
                    //return: userId

                    var description = item
                                      ?.Children
                                      .Where(child => child.GetAttribute("class") == "tc-desc")
                                      .FirstOrDefault()
                                      ?.TextContent
                                      ?.Trim()
                                      ?.ToLower();

                    //_parserManager.AddUpdateItem(userId, gameId, description);
                    //return: itemId

                    var price = item
                                .Children
                                .Where(child => child.GetAttribute("class") == "tc-price")
                                .Select(item =>
                                {
                                    double.TryParse(
                                        item.GetAttribute("data-s"),
                                        NumberStyles.Number,
                                        CultureInfo.InvariantCulture,
                                        out double cost);

                                    return cost;
                                })
                                .FirstOrDefault();

                    var count = item
                                .Children
                                .Where(child => child.GetAttribute("class").Contains("tc-amount"))
                                .Select(item =>
                                {
                                    int.TryParse(
                                        item.TextContent,
                                        NumberStyles.Number,
                                        CultureInfo.InvariantCulture,
                                        out int cost);

                                    return cost;
                                })
                                .FirstOrDefault();

                    //_parserManager.AddUpdateItemPrice(itemId, price, count);
                    //return: itemId
                }
            }
        }
    }
}