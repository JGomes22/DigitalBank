using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalBank.Application.AppServices.Interfaces
{
    public interface ILancamentoAppService : IAppServiceBase<Lancamento>
    {
        Task<HttpResult<string>> CreateAsync(LancamentoRequest lancamentoRequest, CancellationToken cancellationToken);
    }
}
