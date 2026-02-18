using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public interface IRepository<T> where T : IEntity
    {
        Task CreateItemAsync(T item);
        Task<T?> GetItemAsync(Guid id);
        Task<IReadOnlyCollection<T>> GetItemsAsync();
        Task UpdateItemAsync(T item);
        Task DeleteItemAsync(Guid id);
    }
}