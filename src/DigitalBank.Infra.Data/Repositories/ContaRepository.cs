using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Interfaces.Contexts;
using DigitalBank.Domain.Interfaces.Repositories;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace DigitalBank.Infra.Data.Repositories
{
    public class ContaRepository : Repository<Conta>, IContaRepository
    {
        public ContaRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<bool> AtualizaContaAsync(Conta conta)
        {
            var updateBuilder = Builders<Conta>.Update.Set(x => x.Numero, conta.Numero);
            var result = await Collection.UpdateOneAsync(x => x.Id == conta.Id, updateBuilder);
            return result.ModifiedCount == 1;
        }

        public async Task<bool> AtualizaSaldoAsync(string id, decimal valor)
        {
            var updateBuilder = Builders<Conta>.Update.Set(x => x.Saldo, valor);
            var result = await Collection.UpdateOneAsync(x => x.Id == id, updateBuilder);
            return result.ModifiedCount == 1;
        }
    }
}
