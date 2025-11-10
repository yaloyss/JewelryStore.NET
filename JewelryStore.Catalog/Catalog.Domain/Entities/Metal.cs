namespace JewelryStore.CatalogService.Catalog.Domain.Entities
{
	public class Metal
	{
		public int MetalId { get; set; }

		public string Name { get; set; } = null!;

        public string? Color { get; set; } = null!;
    }
}

