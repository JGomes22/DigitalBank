using DigitalBank.Application.AppServices.Interfaces;
using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalBank.Application.AppServices
{
    public class ContaAppService : AppServiceBase<Conta>, IContaAppService
    {
        private readonly IContaService _contaService;
        public ContaAppService(IContaService contaService) : base(contaService)
        {
            _contaService = contaService;
        }

        public async Task<HttpResult<string>> CreateAsync(ContaRequest contaRequest, CancellationToken cancellationToken)
        {
            return await _contaService.CreateAsync(contaRequest, cancellationToken);
        }
    }
}
