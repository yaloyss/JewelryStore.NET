namespace Catalog.DAL.Sorting
{
	public static class QueryableExtensions
	{
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? orderBy, ISorting<T>? sortHelper)
        {
            if (sortHelper == null || string.IsNullOrWhiteSpace(orderBy))
            {
                return query;
            }
            return sortHelper.ApplySort(query, orderBy);
        }
    }
}

