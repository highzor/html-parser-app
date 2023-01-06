using Web.Html.Parser.App;
using Web.Html.Parser.App.Domain;

internal class Program
{
    private static void Main(string[] args)
    {
        new ParserApplication(new ParserManager()).Start().GetAwaiter().GetResult();
    }
}