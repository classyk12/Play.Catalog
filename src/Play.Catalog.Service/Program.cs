using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Endpoints;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Service.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String)); //store guids as strings in MongoDB
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String)); //store guids as strings in MongoDB

//register configuration settings
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

//define mongoDB connection string
builder.Services.AddSingleton(provider =>
{
    var mongoSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
    var mongoClient = new MongoClient(mongoSettings?.ConnectionString);
    return mongoClient.GetDatabase(serviceSettings?.Name);
});

builder.Services.AddSingleton<IItemsRepository, ItemsRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Catalog.Service v1");
        c.RoutePrefix = string.Empty;
    });
}

// Map minimal API endpoints (Item endpoints)
app.MapItemEndpoints();

app.UseHttpsRedirection();

app.Run();
