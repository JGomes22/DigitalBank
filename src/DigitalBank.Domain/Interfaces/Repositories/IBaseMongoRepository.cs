using DigitalBank.Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalBank.Domain.Interfaces.Repositories
{
    public interface IBaseMongoRepository<T> where T : Entity
    {
        IMongoCollection<T> Collection { get; }
        Task<ICollection<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<string> InsertAsync(T entity, CancellationToken cancellationToken);
        Task<T> GetByIdAsync(string id);
        Task<string> UpdateAsync(Expression<Func<T, bool>> filterDefinition, T entity, ReplaceOptions options);
        Task<long> DeleteAsync(string id, CancellationToken cancellationToken);

    }
}
