using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Enums;
using DigitalBank.Domain.Interfaces.Repositories;
using DigitalBank.Domain.Interfaces.Services;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DigitalBank.Domain.Services
{
    public class ContaService : ServiceBase<Conta>, IContaService
    {
        private readonly IContaRepository _contaRepository;
        public ContaService(IContaRepository contaRepository) : base(contaRepository)
        {
            _contaRepository = contaRepository;
        }

        public async Task<HttpResult<bool>> AtualizaContaAsync(ContaRequest contaRequest)
        {
            if (contaRequest == null)
                return new HttpResult<bool>(false, HttpStatusCode.BadRequest, Error.GenerateFailure("conta não pode ser nula."));

            var conta = _mapper.Map<ContaRequest, Conta>(contaRequest);
            if (!conta.IsValid(EValidationStage.Update))
                return new HttpResult<bool>(false, HttpStatusCode.BadRequest, conta.ValidationErrors);

            var result = await _contaRepository.AtualizaContaAsync(conta);
            if (!result)
                return new HttpResult<bool>(false, HttpStatusCode.NotFound, Error.GenerateFailure("Não foi possível atualizar a conta."));

            return new HttpResult<bool>(true, HttpStatusCode.OK, null);
        }

        public async Task<HttpResult<bool>> AtualizaSaldoAsync(string id, decimal valor)
        {
            var saldoResult = await _contaRepository.AtualizaSaldoAsync(id, valor);
            if (!saldoResult)
                return new HttpResult<bool>(false, HttpStatusCode.InternalServerError, Error.GenerateFailure("Erro ao atualizar saldo."));

            return new HttpResult<bool>(true, HttpStatusCode.OK, null);
        }

        public Task<HttpResult<string>> GerarNumeroAsync()
        {
            try
            {
                var accountNumber = new Random().Next(0, 70000).ToString().PadLeft(6, '0');
                var formatedNumber = $"{accountNumber.Substring(0, accountNumber.Length - 1)}-{accountNumber.Substring(accountNumber.Length - 1)}";
                return Task.FromResult(new HttpResult<string>(formatedNumber, HttpStatusCode.OK, null));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new HttpResult<string>(null, HttpStatusCode.BadRequest, Error.GenerateFailure($"Erro ao gerar o número da conta: {ex.Message}")));
            }
        }
    }
}
