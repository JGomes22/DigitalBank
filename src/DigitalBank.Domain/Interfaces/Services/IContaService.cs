using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.Entities;
using System.Threading.Tasks;

namespace DigitalBank.Domain.Interfaces.Services
{
    public interface IContaService : IServiceBase<Conta>
    {
        Task<HttpResult<string>> GerarNumeroAsync();
        Task<HttpResult<bool>> AtualizaSaldoAsync(string id, decimal valor);
        Task<HttpResult<bool>> AtualizaContaAsync(ContaRequest contaRequest);
    }
}
