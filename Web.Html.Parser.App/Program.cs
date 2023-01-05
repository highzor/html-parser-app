using Web.Html.Parser.App;
using Web.Html.Parser.App.Domain;
using Microsoft.Extensions.Configuration;
using Web.Html.Parser.App.Models.Configuration;

internal class Program
{
    private static void Main(string[] args)
    {
        new ParserApplication(new ParserManager()).Start().GetAwaiter().GetResult();
    }
}