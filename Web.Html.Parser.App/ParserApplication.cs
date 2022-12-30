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
        var document = await _parserManager.GetDocument(url);
        var games = document.All.Where(letter => letter.GetAttribute("class") == "promo-game-item").ToList();
    }
}