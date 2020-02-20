using DigitalBank.Domain.Entities;
using System.Threading.Tasks;

namespace DigitalBank.Domain.Interfaces.Repositories
{
    public interface IContaRepository : IBaseMongoRepository<Conta>
    {
        Task<bool> AtualizaContaAsync(Conta conta);
        Task<bool> AtualizaSaldoAsync(string id, decimal valor);
    }
}
