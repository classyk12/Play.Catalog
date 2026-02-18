using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public class MongoRepository<T>(IMongoDatabase database) : IRepository<T> where T : IEntity
    {
        // derive the collection name automatically from the entity type
        private static string CollectionName => typeof(T).Name.ToLowerInvariant() + "s";
        private readonly IMongoCollection<T>? itemsCollection = database.GetCollection<T>(CollectionName);
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter; //helps us build the queries

        public async Task CreateItemAsync(T item)
        {
            ArgumentNullException.ThrowIfNull(item);
            await itemsCollection!.InsertOneAsync(item);
        }

        public async Task<T?> GetItemAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            return await itemsCollection!.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<T>> GetItemsAsync()
        {
            return await itemsCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task UpdateItemAsync(T item)
        {
            ArgumentNullException.ThrowIfNull(item);
            var filter = filterBuilder.Eq(existingItem => existingItem.Id, item.Id) ?? throw new Exception($"Item with id {item.Id} not found.");
            await itemsCollection!.ReplaceOneAsync(filter, item);
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            await itemsCollection!.DeleteOneAsync(filter);
        }
    }

}