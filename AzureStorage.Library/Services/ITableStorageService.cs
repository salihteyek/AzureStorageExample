using Azure;
using System.Linq.Expressions;

namespace AzureStorage.Library.Services
{
    public interface ITableStorageService<TEntity>
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> FindAsync(string partitionKey, string rowKey);
        Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> query);
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity> UpdateAsync(TEntity entity, ETag eTag);
        Task<TEntity> UpsertAsync(TEntity entity);
        Task DeleteAsync(string partitionKey, string rowKey);
    }
}
