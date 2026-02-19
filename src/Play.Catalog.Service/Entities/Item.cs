using Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Play.Catalog.Service.Entities
{
    public class Item : IEntity
    {
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}