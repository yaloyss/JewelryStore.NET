namespace JewelryStore.Catalog.Client;

public class CatalogClientOptions
{
    public const string SectionName = "CatalogService";

    public string BaseUrl { get; set; } = "http://localhost:5209";
}
