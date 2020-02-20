using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalBank.Application.AppServices.Interfaces
{
    public interface IContaAppService : IAppServiceBase<Conta>
    {
        Task<HttpResult<string>> CreateAsync(ContaRequest contaRequest, CancellationToken cancellationToken);
    }
}
