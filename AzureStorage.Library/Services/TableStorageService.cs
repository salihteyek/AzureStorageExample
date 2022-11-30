using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace AzureStorage.Library.Services
{
    public class TableStorageService<TEntity> : ITableStorageService<TEntity> where TEntity : class, ITableEntity, new()
    {
        private readonly TableServiceClient _serviceClient;
        private readonly TableClient _tableClient;
        private readonly AzureConnectionConfigs _azureConfigs;
        public TableStorageService(IOptions<AzureConnectionConfigs> azureConfigs)
        {
            _azureConfigs = azureConfigs.Value;
            _serviceClient = new TableServiceClient(_azureConfigs.StorageConStr);
            _tableClient = _serviceClient.GetTableClient(typeof(TEntity).Name);
            //_tableClient.CreateIfNotExists();
        }

        private async Task<TableClient> GetTableClientAsync()
        {
            await _tableClient.CreateIfNotExistsAsync();
            return _tableClient;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var tableClient = await GetTableClientAsync();
            await tableClient.AddEntityAsync(entity);
            return entity;
        }

        public async Task DeleteAsync(string partitionKey, string rowKey)
        {
            var tableClient = await GetTableClientAsync();
            await tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task<TEntity> FindAsync(string partitionKey, string rowKey)
        {
            var tableClient = await GetTableClientAsync();
            var response = await tableClient.GetEntityAsync<TEntity>(partitionKey, rowKey);
            return response.Value;
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
			var tableClient = await GetTableClientAsync();
			return await tableClient.QueryAsync<TEntity>(maxPerPage: 100).ToListAsync();
        }

        public async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> query)
        {
            var tableClient = await GetTableClientAsync();
            var response = await tableClient.QueryAsync<TEntity>(filter: query).ToListAsync();
            return response;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, ETag eTag)
        {
            var tableClient = await GetTableClientAsync();
			await tableClient.UpdateEntityAsync(entity, ETag.All);
			return entity;
        }

        public async Task<TEntity> UpsertAsync(TEntity entity)
        {
            var tableClient = await GetTableClientAsync();
            await tableClient.UpsertEntityAsync<TEntity>(entity);
            return entity;
        }
    }
}
