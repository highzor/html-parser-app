using Web.Html.Parser.App.Domain;

namespace Web.Html.Parser.App;

public class ParserApplication
{
    private readonly IParserManager _parserManager;

    public ParserApplication(IParserManager parserManager)
    {
        _parserManager = parserManager;
    }
    public async Task Start(string url)
    {
        {
            var document = await _parserManager.GetDocument(url);
            var games = _parserManager.GetGames(document);


            foreach (var game in games)
            {
                var gameId = await _parserManager.AddGetGame(game);
                var gameItems = _parserManager.GetGameItems(game);

                foreach (var gameItem in gameItems)
                {
                    var gameItemHref = await _parserManager.AddGameItem(gameId, gameItem);
                    document = await _parserManager.GetDocument(gameItemHref);
                    var items = _parserManager.GetItems(document);

                    foreach (var item in items)
                    {
                        var userId = await _parserManager.AddUpdateUser(item);
                        var description = _parserManager.GetItemDescription(item);
                        var itemId = await _parserManager.AddUpdateItem(userId, gameId, description);
                        var itemPriceId = _parserManager.AddUpdateItemPrice(itemId, item);
                    }
                }
            }
        }
    }
}