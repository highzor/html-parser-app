namespace Web.Html.Parser.App.Models;
public class ItemPriceModel
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public double Price { get; set; }
    public int Count { get; set; }
    public DateTime DateTimeUpdate { get; set; }
}
