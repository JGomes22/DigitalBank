using MongoDB.Driver;

namespace DigitalBank.Domain.Interfaces.Contexts
{
    public interface IMongoContext
    {
        string ConnectionString { get; }
        MongoClient Client { get; }
        IMongoDatabase Database { get; }
        string GetCollectionName<T>();
        IMongoCollection<T> GetCollection<T>(string name);
        IMongoCollection<T> GetCollection<T>();
    }
}
