using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Interfaces.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalBank.Infra.Data.Repositories
{
    public abstract class BaseMongoRepository<T> : IBaseMongoRepository<T> where T : Entity
    {
        public abstract IMongoCollection<T> Collection { get; }

        public async Task<long> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            var delete = await Collection.DeleteOneAsync(x => x.Id.Equals(id), cancellationToken);
            return delete.DeletedCount;
        }

        public async Task<ICollection<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await Collection.Find(predicate).ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await Collection.Find(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<string> InsertAsync(T entity, CancellationToken cancellationToken)
        {
            try
            {
                await Collection.InsertOneAsync(entity, new InsertOneOptions { }, cancellationToken);
                return entity.Id;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new Exception($"{typeof(T).Name} '{entity.Id}' já cadastrado", ex);
            }
        }

        public async Task<string> UpdateAsync(Expression<Func<T, bool>> filterDefinition, T entity, ReplaceOptions options)
        {
            var upsert = await Collection.ReplaceOneAsync(filterDefinition, entity, options);
            if (upsert.IsAcknowledged)
                return entity.Id;
            else
                return default;
        }
    }
}
