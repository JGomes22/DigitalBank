using DigitalBank.Application.AppServices.Interfaces;
using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Enums;
using DigitalBank.Domain.Interfaces.Services;
using DigitalBank.Infra.CrossCutting.Extensions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalBank.Application.AppServices
{
    public class LancamentoAppService : AppServiceBase<Lancamento>, ILancamentoAppService
    {
        private readonly ILancamentoService _lancamentoService;
        public LancamentoAppService(ILancamentoService lancamentoService) : base(lancamentoService)
        {
            _lancamentoService = lancamentoService;
        }

        public async Task<HttpResult<string>> CreateAsync(LancamentoRequest lancamentoRequest, CancellationToken cancellationToken)
        {
            if (lancamentoRequest == null)
                return new HttpResult<string>(null, HttpStatusCode.BadRequest, Error.GenerateFailure("Lançamento não pode ser nulo."));

            HttpResult<decimal> result;
            switch (lancamentoRequest.Operacao)
            {
                case EOperacao.Deposito:
                    {
                        result = await _lancamentoService.DepositarAsync(lancamentoRequest);
                        break;
                    }
                case EOperacao.Saque:
                    {
                        result = await _lancamentoService.SacarAsync(lancamentoRequest);
                        break;
                    }
                case EOperacao.Transferencia:
                    {
                        result = await _lancamentoService.TransferirAsync(lancamentoRequest);
                        break;
                    }
                default:
                    return new HttpResult<string>(null, HttpStatusCode.BadRequest, Error.GenerateFailure("Operação não suportada."));
            }

            if (!result.StatusCode.IsSuccess())
                return new HttpResult<string>(null, result.StatusCode, result.Errors);

            return await _lancamentoService.CreateAsync(lancamentoRequest, cancellationToken);
        }
    }
}
