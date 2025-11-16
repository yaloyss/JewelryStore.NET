namespace Catalog.Domain.Entities
{
	public class ProductStone
	{
		public int ProductId { get; set; }

        public int StoneId { get; set; }

		public Product Product { get; set; } = null!;
        public Stone Stone { get; set; } = null!;
    }
}

