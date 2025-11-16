namespace Catalog.Domain.Entities
{
    public class Stone
	{
        public int StoneId { get; set; }

        public string Name { get; set; } = null!;

        public ICollection<ProductStone> ProductStones { get; set; } = new List<ProductStone>();
    }
}

