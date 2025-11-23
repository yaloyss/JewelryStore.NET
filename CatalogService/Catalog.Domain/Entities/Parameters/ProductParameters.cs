namespace Catalog.Domain.Entities.Parameters
{
    public class ProductParameters : QueryParameters
    {
        public int? CategoryId { get; set; }
        public int? MetalId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SearchName { get; set; }
    }
}

