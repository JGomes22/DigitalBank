using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Interfaces.Contexts;
using MongoDB.Driver;

namespace DigitalBank.Infra.Data.Repositories
{
    public class Repository<T> : BaseMongoRepository<T> where T : Entity
    {
        private readonly dynamic _context;

        public Repository(IMongoContext context)
        {
            _context = context;
        }

        public override IMongoCollection<T> Collection => _context.GetCollection<T>();
    }
}
