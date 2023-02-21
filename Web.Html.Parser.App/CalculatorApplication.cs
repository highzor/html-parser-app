using Web.Html.Parser.App.Domain;

namespace Web.Html.Parser.App;

public class CalculatorApplication
{
    private readonly ICalculatorManager _calculatorManager;

    public CalculatorApplication(ICalculatorManager calculatorManager)
    {
        _calculatorManager = calculatorManager;
    }
    public async Task Start()
    {
        var result = await _calculatorManager.GetItemsPrice();
        var stop = "";
    }
}