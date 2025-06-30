namespace CashFlow.BuildingBlocks.Domain.Caching
{
    public interface IRedisCacheService
    {
        Task AddOrUpdateAsync<T>(string key, T item);
        Task<IEnumerable<T>?> GetItemsAsync<T>(string key);
        Task<IEnumerable<T>?> GetItemsByPatternAsync<T>(string pattern);
        Task<IEnumerable<T>?> GetAllItemsAsync<T>();

        Task DeleteAsync(string key);
    }
}