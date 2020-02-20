using DigitalBank.Domain.Interfaces.Contexts;
using DigitalBank.Infra.CrossCutting.Ioc;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;

namespace DigitalBank.Infra.Data.Context
{
    public class MongoContext : IMongoContext
    {
        public MongoContext()
        {
            ConnectionString = Bootstrapper._mongoConnection;
        }

        public string ConnectionString { get; }

        public MongoClient Client => new MongoClient(new MongoUrl(ConnectionString));

        public IMongoDatabase Database => Client.GetDatabase("DigitalBank");

        public IMongoCollection<T> GetCollection<T>(string name) => Database.GetCollection<T>(name);

        public IMongoCollection<T> GetCollection<T>() => GetCollection<T>(GetCollectionName<T>());

        public string GetCollectionName<T>()
        {
            if (Attribute.GetCustomAttribute(typeof(T), typeof(BsonDiscriminatorAttribute)) != null)
            {
                var cm = BsonClassMap.LookupClassMap(typeof(T));
                if (!string.IsNullOrWhiteSpace(cm.Discriminator))
                    return cm.Discriminator;
            }

            return typeof(T).Name;
        }

    }
}
