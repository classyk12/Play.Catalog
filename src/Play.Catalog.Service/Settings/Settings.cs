namespace Play.Catalog.Service.Settings
{
    public class MongoDbSettings
    {
        //since the value of Host and Port is not expected to change during the execution of the application, we can use init only properties
        public string? Host { get; init; }
        public int Port { get; init; }
        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }

    public class ServiceSettings
    {
        public string? Name { get; set; }
    }
}