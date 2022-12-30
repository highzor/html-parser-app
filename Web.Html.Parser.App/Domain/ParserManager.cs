using AngleSharp;
using AngleSharp.Dom;

namespace Web.Html.Parser.App.Domain;

public interface IParserManager
{
    Task<IDocument> GetDocument(string url);
}

public class ParserManager : IParserManager
{
    public async Task<IDocument> GetDocument(string url)
    {
        var config = Configuration.Default.WithDefaultLoader();
        return await BrowsingContext.New(config).OpenAsync(url);
    }
}

