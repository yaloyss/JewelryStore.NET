namespace Reviews.Infrastructure.Mongo.Seeder
{
	public interface IDataSeeder
	{
		Task SeedAsync(CancellationToken cancellationToken = default);
	}
}

