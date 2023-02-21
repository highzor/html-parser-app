using Web.Html.Parser.App.Data;
using Web.Html.Parser.App.Models;

namespace Web.Html.Parser.App.Domain;

public interface ICalculatorManager
{
    Task<IList<ItemPriceModel>> GetItemsPrice();
}

public class CalculatorManager : ICalculatorManager
{
    private readonly ICalculatorRepository _calculatorRepository;

    public CalculatorManager(ICalculatorRepository calculatorRepository)
    {
        _calculatorRepository = calculatorRepository;
    }

    public async Task<IList<ItemPriceModel>> GetItemsPrice()
    {
        try
        {
            return await _calculatorRepository.GetItemsPrice();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
}