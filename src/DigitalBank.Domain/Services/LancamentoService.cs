using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.DTO.Response;
using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Enums;
using DigitalBank.Domain.Interfaces.Repositories;
using DigitalBank.Domain.Interfaces.Services;
using DigitalBank.Infra.CrossCutting.Extensions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DigitalBank.Domain.Services
{
    public class LancamentoService : ServiceBase<Lancamento>, ILancamentoService
    {
        private readonly IContaService _contaService;
        public LancamentoService(ILancamentoRepository lancamentoRepository, IContaService contaService) : base(lancamentoRepository)
        {
            _contaService = contaService;
        }

        private Task<HttpResult<decimal>> CheckOperation(LancamentoRequest lancamentoRequest)
        {
            var lancamento = _mapper.Map<LancamentoRequest, Lancamento>(lancamentoRequest);
            if (!lancamento.IsValid(EValidationStage.Create))
                return Task.FromResult(new HttpResult<decimal>(0, HttpStatusCode.BadRequest, lancamento.ValidationErrors));

            return Task.FromResult(new HttpResult<decimal>(0, HttpStatusCode.OK, null));
        }

        public async Task<HttpResult<decimal>> DepositarAsync(LancamentoRequest lancamentoRequest)
        {
            try
            {
                var validacao = await CheckOperation(lancamentoRequest);
                if (!validacao.StatusCode.IsSuccess())
                    return validacao;

                var contaDestino = await _contaService.GetByIdAsync<ContaResponse>(lancamentoRequest.IdContaDestino);
                if (!contaDestino.StatusCode.IsSuccess())
                    return new HttpResult<decimal>(0, contaDestino.StatusCode, contaDestino.Errors);

                var conta = _mapper.Map<ContaResponse, Conta>(contaDestino.Value);
                var novoSaldo = await conta.Depositar(lancamentoRequest.Valor);
                if (!novoSaldo.StatusCode.IsSuccess())
                    return new HttpResult<decimal>(0, novoSaldo.StatusCode, novoSaldo.Errors);

                var atualizaSaldoResult = await _contaService.AtualizaSaldoAsync(conta.Id, novoSaldo.Value);
                if (!atualizaSaldoResult.StatusCode.IsSuccess())
                    return new HttpResult<decimal>(0, atualizaSaldoResult.StatusCode, atualizaSaldoResult.Errors);

                return new HttpResult<decimal>(novoSaldo.Value, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return new HttpResult<decimal>(0, HttpStatusCode.InternalServerError, Error.GenerateFailure(ex.Message));
            }
        }

        public async Task<HttpResult<decimal>> SacarAsync(LancamentoRequest lancamentoRequest)
        {
            try
            {
                var validacao = await CheckOperation(lancamentoRequest);
                if (!validacao.StatusCode.IsSuccess())
                    return validacao;

                var contaResponseOrigem = await _contaService.GetByIdAsync<ContaResponse>(lancamentoRequest.IdContaOrigem);
                if (!contaResponseOrigem.StatusCode.IsSuccess())
                    return new HttpResult<decimal>(0, contaResponseOrigem.StatusCode, contaResponseOrigem.Errors);

                var account = _mapper.Map<ContaResponse, Conta>(contaResponseOrigem.Value);
                var newBalance = await account.Sacar(lancamentoRequest.Valor);
                if (!newBalance.StatusCode.IsSuccess())
                    return new HttpResult<decimal>(0, newBalance.StatusCode, newBalance.Errors);

                var atualizaSaldoResult = await _contaService.AtualizaSaldoAsync(account.Id, newBalance.Value);
                if (!atualizaSaldoResult.StatusCode.IsSuccess())
                    return new HttpResult<decimal>(0, atualizaSaldoResult.StatusCode, atualizaSaldoResult.Errors);

                return new HttpResult<decimal>(newBalance.Value, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return new HttpResult<decimal>(0, HttpStatusCode.InternalServerError, Error.GenerateFailure(ex.Message));
            }
        }

        public async Task<HttpResult<decimal>> TransferirAsync(LancamentoRequest lancamentoRequest)
        {
            try
            {

                var validacao = await CheckOperation(lancamentoRequest);
                if (!validacao.StatusCode.IsSuccess())
                    return validacao;

                var contaOrigemResponse = await _contaService.GetByIdAsync<ContaResponse>(lancamentoRequest.IdContaOrigem);
                if (!contaOrigemResponse.StatusCode.IsSuccess())
                    return new HttpResult<decimal>(0, contaOrigemResponse.StatusCode, contaOrigemResponse.Errors);

                var contaDestinoResponse = await _contaService.GetByIdAsync<ContaResponse>(lancamentoRequest.IdContaDestino);
                if (!contaDestinoResponse.StatusCode.IsSuccess())
                    return new HttpResult<decimal>(0, contaDestinoResponse.StatusCode, contaDestinoResponse.Errors);

                var contaOrigem = _mapper.Map<ContaResponse, Conta>(contaOrigemResponse.Value);
                var contaDestino = _mapper.Map<ContaResponse, Conta>(contaDestinoResponse.Value);


                var saldoContaOrigemResult = await contaOrigem.Sacar(lancamentoRequest.Valor);
                if (!saldoContaOrigemResult.StatusCode.IsSuccess())
                    return new HttpResult<decimal>(0, saldoContaOrigemResult.StatusCode, saldoContaOrigemResult.Errors);


                var saldoContaDestinoResult = await contaDestino.Depositar(lancamentoRequest.Valor);
                if (!saldoContaDestinoResult.StatusCode.IsSuccess())
                    return new HttpResult<decimal>(0, saldoContaDestinoResult.StatusCode, saldoContaDestinoResult.Errors);


                var atualizaSaldoResult = await _contaService.AtualizaSaldoAsync(contaOrigem.Id, saldoContaOrigemResult.Value);
                if (!atualizaSaldoResult.StatusCode.IsSuccess())
                    return new HttpResult<decimal>(0, atualizaSaldoResult.StatusCode, atualizaSaldoResult.Errors);

                atualizaSaldoResult = await _contaService.AtualizaSaldoAsync(contaDestino.Id, saldoContaDestinoResult.Value);
                if (!atualizaSaldoResult.StatusCode.IsSuccess())
                    return new HttpResult<decimal>(0, atualizaSaldoResult.StatusCode, atualizaSaldoResult.Errors);

                return new HttpResult<decimal>(saldoContaOrigemResult.Value, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return new HttpResult<decimal>(0, HttpStatusCode.InternalServerError, Error.GenerateFailure(ex.Message));
            }
        }
    }
}
