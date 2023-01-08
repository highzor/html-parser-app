using Web.Html.Parser.App;
using Web.Html.Parser.App.Data;
using Web.Html.Parser.App.Domain;

internal class Program
{
    private static void Main(string[] args)
    {
        new ParserApplication(new ParserManager(new ParserRepository())).Start().GetAwaiter().GetResult();
    }
}