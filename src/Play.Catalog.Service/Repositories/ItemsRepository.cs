using MongoDB.Bson;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public interface IItemsRepository
    {
        Task CreateItemAsync(Item item);
        Task<Item?> GetItemAsync(Guid id);
        Task<IReadOnlyCollection<Item>> GetItemsAsync();
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(Guid id);
    }

    public class ItemsRepository(IMongoDatabase database) : IItemsRepository
    {
        private const string collectionName = "items"; //table/document name
        private readonly IMongoCollection<Item>? itemsCollection = database.GetCollection<Item>(collectionName); //connection to the collection
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter; //helps us build the queries

        public async Task CreateItemAsync(Item item)
        {
            ArgumentNullException.ThrowIfNull(item);
            await itemsCollection!.InsertOneAsync(item);
        }

        public async Task<Item?> GetItemAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            return await itemsCollection!.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<Item>> GetItemsAsync()
        {
            return await itemsCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task UpdateItemAsync(Item item)
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