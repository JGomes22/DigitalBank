using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.Entities;
using System.Threading.Tasks;

namespace DigitalBank.Domain.Interfaces.Services
{
    public interface ILancamentoService : IServiceBase<Lancamento>
    {
        Task<HttpResult<decimal>> SacarAsync(LancamentoRequest lancamentoRequest);
        Task<HttpResult<decimal>> DepositarAsync(LancamentoRequest lancamentoRequest);
        Task<HttpResult<decimal>> TransferirAsync(LancamentoRequest lancamentoRequest);
    }
}
