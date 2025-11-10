using JewelryStore.CatalogService.Catalog.DAL.Repositories.Interfaces;

namespace JewelryStore.CatalogService.Catalog.DAL.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IMetalRepository Metals { get; }
        IStoneRepository Stones { get; }
        IProductStoneRepository ProductStones { get; }

        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}

