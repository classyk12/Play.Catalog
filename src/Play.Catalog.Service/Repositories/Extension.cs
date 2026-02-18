using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Settings;

namespace Play.Catalog.Service.Repositories
{
    public static class Extensions
    {
        public static void AddMongoServices(this IServiceCollection services)
        {
            // Register services
            BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String)); //store guids as strings in MongoDB
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String)); //store guids as strings in MongoDB

            //register configuration settings
            var serviceSettings = services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            //define mongoDB connection string
            services.AddSingleton(provider =>
            {
                var mongoSettings = provider.GetRequiredService<IConfiguration>().GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoSettings?.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings?.Name);
            });
        }
    }
}