namespace Catalog.DAL.Pagination
{
	public class PagedResponse<T>
	{
        public List<T> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }

        public PagedResponse() { }

        public static PagedResponse<TDto> FromPagedList<TEntity, TDto>(PagedList<TEntity> pagedList, List<TDto> mappedItems)
        {
            return new PagedResponse<TDto>
            {
                Items = mappedItems,
                CurrentPage = pagedList.CurrentPage,
                PageSize = pagedList.PageSize,
                TotalCount = pagedList.TotalCount,
                TotalPages = pagedList.TotalPages,
                HasNext = pagedList.HasNext,
                HasPrevious = pagedList.HasPrevious
            };
        }
    }
}

