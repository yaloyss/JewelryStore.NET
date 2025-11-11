using Catalog.DAL.Repositories.Interfaces;

namespace Catalog.DAL.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IMetalRepository Metals { get; }
        IStoneRepository Stones { get; }
        IProductStoneRepository ProductStones { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}

