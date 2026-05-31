namespace JewelryStore.Catalog.Client;

public class CatalogProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Weight { get; set; }
    public decimal? Size { get; set; }
    public string? Manufacturer { get; set; }
    public int? MetalId { get; set; }
    public int CategoryId { get; set; }
}
