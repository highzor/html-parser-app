using AngleSharp;
using AngleSharp.Dom;
using System.Globalization;
using Web.Html.Parser.App.Data;

namespace Web.Html.Parser.App.Domain;

public interface IParserManager
{
    Task<IDocument> GetDocument(string url);
    IList<IElement> GetGames(IDocument document);
    Task<Guid> AddGetGame(IElement game);
    IList<IElement> GetGameItems(IElement game);
    Task<string> AddGameItem(Guid gameId, IElement gameItem);
    IList<IElement> GetItems(IDocument document);
    Task<Guid> AddGetUser(IElement item);
    string GetItemDescription(IElement item);
    Task<Guid> AddGetItem(Guid userId, Guid gameId, string description);
    Task<Guid> AddGetItemPrice(Guid itemId, IElement item);
}

public class ParserManager : IParserManager
{
    private readonly IParserRepository _parserRepository;

    public ParserManager(IParserRepository parserRepository)
    {
        _parserRepository = parserRepository;
    }

    public async Task<IDocument> GetDocument(string url)
    {
        try
        {
            var config = Configuration.Default.WithDefaultLoader();
            return await BrowsingContext.New(config).OpenAsync(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public IList<IElement> GetGames(IDocument document)
    {
        try
        {
            return document
                   .All
                   .Where(letter => letter.GetAttribute("class") == "promo-game-item")
                   .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task<Guid> AddGetGame(IElement game)
    {
        try
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
            return await _parserRepository.AddGetGame(gameWebId, gameName);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Guid.Empty;
        }
    }

    public IList<IElement> GetGameItems(IElement game)
    {
        try
        {
            return game
                   .Children
                   .Where(child => child.LocalName == "ul")
                   .FirstOrDefault()
                   ?.Children
                   .Where(child => child.LocalName == "li")
                   .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
    public async Task<string> AddGameItem(Guid gameId, IElement gameItem)
    {
        try
        {
            var gameItemLinkElement = gameItem
                                      .Children
                                      .Where(child => child.LocalName == "a")
                                      .FirstOrDefault();

            var gameItemHref = gameItemLinkElement?.GetAttribute("href");
            var gameItemTitle = gameItemLinkElement
                                ?.TextContent
                                .Trim();

            var gameItemId = await _parserRepository.AddGameItem(gameId, gameItemTitle);

            return gameItemHref;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return string.Empty;
        }
    }

    public IList<IElement> GetItems(IDocument document)
    {
        try
        {
            return document
                   .All
                   .Where(letter => letter.GetAttribute("class") == "tc-item")
                   .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task<Guid> AddGetUser(IElement item)
    {
        try
        {
            var userWebId = getUserWebIdFromHref(item);

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

            return await _parserRepository.AddGetUser(userWebId, userName);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Guid.Empty;
        }
    }

    public string GetItemDescription(IElement item)
    {
        try
        {
            var description = string.Empty;
            var descriptionElement = item
                                     ?.Children
                                     .Where(child => child.GetAttribute("class") == "tc-desc")
                                     .FirstOrDefault();

            if (descriptionElement is not null)
            {
                description = descriptionElement
                              ?.TextContent
                              ?.Trim()
                              ?.ToLower();
            }
            else
            {
                description = item
                              ?.Children
                              .Where(child => child.GetAttribute("class").Contains("tc-server"))
                              .FirstOrDefault()
                              ?.TextContent
                              ?.Trim()
                              ?.ToLower();

                description += @$" -> {item
                                       ?.Children
                                       .Where(child => child.GetAttribute("class").Contains("tc-side"))
                                       .FirstOrDefault()
                                       ?.TextContent
                                       ?.Trim()
                                       ?.ToLower()}";
            }

            return description;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return string.Empty;
        }
    }

    public async Task<Guid> AddGetItem(Guid userId, Guid gameId, string description)
    {
        try
        {
            return await _parserRepository.AddGetItem(userId, gameId, description);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Guid.Empty;
        }
    }

    public async Task<Guid> AddGetItemPrice(Guid itemId, IElement item)
    {
        try
        {
            var isSingle = false;
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

            if (count == 0)
            {
                isSingle = true;
                count = 1;
            }

            return await _parserRepository.AddGetItemPrice(itemId, price, isSingle, count);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Guid.Empty;
        }
    }

    private int getUserWebIdFromHref(IElement item)
    {
        var userUrl = item
                      ?.Children
                      .Where(child => child.GetAttribute("class") == "tc-user")
                      .FirstOrDefault()
                      ?.Children
                      .Where(child => child.GetAttribute("class").Contains("media-user"))
                      .FirstOrDefault()
                      ?.Children
                      .Where(child => child.GetAttribute("class") == "media-left")
                      .FirstOrDefault()
                      ?.Children
                      .FirstOrDefault()
                      ?.GetAttribute("data-href");

        int.TryParse(
            userUrl
            .Split("users/")?[1]
            .Trim('/'),
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out int userWebId);

        return userWebId;
    }
}