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
        try
        {
            var config = Configuration.Default.WithDefaultLoader();
            return await BrowsingContext.New(config).OpenAsync(url);
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}

