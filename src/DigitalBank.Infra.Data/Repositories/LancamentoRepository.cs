using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Interfaces.Contexts;
using DigitalBank.Domain.Interfaces.Repositories;

namespace DigitalBank.Infra.Data.Repositories
{
    public class LancamentoRepository : Repository<Lancamento>, ILancamentoRepository
    {
        public LancamentoRepository(IMongoContext context) : base(context)
        {
        }
    }
}
