using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Catalog.DAL.Sorting
{
    public class Sorting<T> : ISorting<T>
    {
        public IQueryable<T> ApplySort(IQueryable<T> entities, string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                return entities;
            }

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                var propertyFromQueryName = param.Trim().Split(' ')[0];
                var descending = param.Trim().EndsWith(" desc", StringComparison.OrdinalIgnoreCase);

                var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.OrdinalIgnoreCase));

                if (objectProperty == null)
                    continue;

                var direction = descending ? "descending" : "ascending";
                orderQueryBuilder.Append($"{objectProperty.Name} {direction}, ");
            }
            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

            if (string.IsNullOrWhiteSpace(orderQuery))
            {
                return entities;
            }
            return entities.OrderBy(orderQuery);
        }
    }
}

