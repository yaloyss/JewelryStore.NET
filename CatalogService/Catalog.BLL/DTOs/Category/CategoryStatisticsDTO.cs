namespace Catalog.BLL.DTOs.Category
{
	public class CategoryStatisticsDTO
	{
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public int TotalProducts { get; set; }
        public int GoldenProducts { get; set; }
        public int SilverProducts { get; set; }
    }
}

