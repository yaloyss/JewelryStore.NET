using Catalog.DAL.Data;
using Catalog.DAL.Repositories;
using Catalog.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Catalog.DAL.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CatalogDbContext _context;
        private IDbContextTransaction? _transaction;

        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }
        public IMetalRepository Metals { get; }
        public IStoneRepository Stones { get; }
        public IProductStoneRepository ProductStones { get; }

        public UnitOfWork(CatalogDbContext context)
        {
            _context = context;

            Products = new ProductRepository(_context);
            Categories = new CategoryRepository(_context);
            Metals = new MetalRepository(_context);
            Stones = new StoneRepository(_context);
            ProductStones = new ProductStoneRepository(_context);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);

                if (_transaction != null)
                {
                    await _transaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}

