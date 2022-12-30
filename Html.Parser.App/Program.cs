using AngleSharp.Html.Parser;
using System.Globalization;

internal class Program
{
    private static void Main(string[] args)
    {
        var folders = Directory.GetDirectories("../../../HtmlPages/");
        
        foreach (var folder in folders)
        {
            Console.WriteLine(Path.GetFileName(folder));

            var htmlPages = Directory.GetFiles(folder).Select(page => Path.GetFileName(page)).ToList();

            foreach (var htmlPage in htmlPages)
            {
                var html = File.ReadAllText($"{folder}/{htmlPage}");

                var parser = new HtmlParser();
                var document = parser.ParseDocument(html);

                var pageName = document.All.Where(item => item.GetAttribute("class") == "block-info").FirstOrDefault()?.ParentElement?.ParentElement?.Children[1].TextContent;
                var prices = document.All
                .Where(item => item.GetAttribute("class") == "tc-price").Select(item =>
                {
                    double.TryParse(item.GetAttribute("data-s"), NumberStyles.Number, CultureInfo.InvariantCulture, out double cost);
                    return cost;
                }).ToList();

                var summ = 0.0;
                foreach (var price in prices)
                {
                    summ += price;
                }

                Console.WriteLine(@$"
            {pageName}
            Сумма позиций: {summ} руб.
            Кол-во позиций: {prices.Count} шт.
            сумма / кол-во позиций = {summ / prices.Count} руб.
            ");
            }
        }

        Console.ReadLine();
    }
}