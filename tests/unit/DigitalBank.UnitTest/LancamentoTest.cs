using DigitalBank.Application.AppServices;
using DigitalBank.Application.AppServices.Interfaces;
using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.DTO.Response;
using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Enums;
using DigitalBank.Domain.Interfaces.Repositories;
using DigitalBank.Domain.Interfaces.Services;
using DigitalBank.Domain.Services;
using MongoDB.Driver;
using NSubstitute;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DigitalBank.UnitTest
{
    public class LancamentoTest
    {
        private readonly ILancamentoAppService _lancamentoAppService;
        private readonly IContaService _contaService;
        private readonly IContaRepository _contaRepository;
        private readonly ILancamentoService _lancamentoService;
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public LancamentoTest()
        {
            _lancamentoRepository = Substitute.For<ILancamentoRepository>();
            _contaRepository = Substitute.For<IContaRepository>();
            _contaService = new ContaService(_contaRepository);
            _lancamentoService = new LancamentoService(_lancamentoRepository, _contaService);
            _lancamentoAppService = new LancamentoAppService(_lancamentoService);
            _cancellationToken = new CancellationToken();
            Setup();
        }

        private void Setup()
        {

            _lancamentoRepository.InsertAsync(null, Arg.Any<CancellationToken>()).Returns(Task.FromResult(string.Empty));
            _lancamentoRepository.InsertAsync(Arg.Is<Lancamento>(x => x.Id == "000000"), Arg.Any<CancellationToken>()).Returns(Task.FromResult(string.Empty));
            _lancamentoRepository.InsertAsync(Arg.Is<Lancamento>(x => x.Id == "000001"), Arg.Any<CancellationToken>()).Returns(Task.FromResult("000001"));


            _lancamentoRepository.GetByIdAsync("000002").Returns(Task.FromResult(default(Lancamento)));
            _lancamentoRepository.GetByIdAsync("000003").Returns(Task.FromResult(new Lancamento()
            {
                Id = "000003",
                Created = DateTime.UtcNow,
                Operacao = EOperacao.Transferencia,
                IdContaDestino = "000033",
                IdContaOrigem = "000044",
                Valor = 100
            }));

            _lancamentoRepository.UpdateAsync(Arg.Any<Expression<Func<Lancamento, bool>>>(), Arg.Is<Lancamento>(x => x.Id == "000077"), Arg.Any<ReplaceOptions>()).Returns(Task.FromResult(""));
            _lancamentoRepository.UpdateAsync(Arg.Any<Expression<Func<Lancamento, bool>>>(), Arg.Is<Lancamento>(x => x.Id == "000099"), Arg.Any<ReplaceOptions>()).Returns(Task.FromResult("1"));


            _contaRepository.GetByIdAsync(Arg.Is<string>("000100")).Returns(new Conta()
            {
                Id = "000100",
                Numero = "0102030",
                Created = DateTime.UtcNow,
            });

            _contaRepository.AtualizaSaldoAsync("000100", Arg.Any<decimal>()).Returns(true);

            _contaRepository.GetByIdAsync(Arg.Is<string>("000101")).Returns(new Conta(100)
            {
                Id = "000101",
                Numero = "0102030",
                Created = DateTime.UtcNow,
            });

            _contaRepository.AtualizaSaldoAsync("000101", Arg.Any<decimal>()).Returns(false);

            _contaRepository.GetByIdAsync(Arg.Is<string>("000102")).Returns(new Conta(100)
            {
                Id = "000102",
                Numero = "0102030",
                Created = DateTime.UtcNow,
            });

            _contaRepository.AtualizaSaldoAsync("000102", Arg.Any<decimal>()).Returns(true);


            _contaRepository.GetByIdAsync(Arg.Is<string>("3")).Returns(new Conta(100)
            {
                Id = "000103",
                Numero = "0102030",
                Created = DateTime.UtcNow,
            });
            _contaRepository.GetByIdAsync("0").Returns(default(Conta));
            _contaRepository.AtualizaSaldoAsync("000103", Arg.Any<decimal>()).Returns(true);


            _lancamentoRepository.DeleteAsync("000055", Arg.Any<CancellationToken>()).Returns(Task.FromResult(long.Parse("1")));
            _lancamentoRepository.DeleteAsync("000066", Arg.Any<CancellationToken>()).Returns(Task.FromResult(long.Parse("0")));

        }

        [Fact]
        public async void TestCreateLancamentoObjectNull()
        {
            var result = await _lancamentoAppService.CreateAsync<LancamentoRequest>(null, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Objeto não pode ser nulo.");
        }

        [Fact]
        public async void TesteCreateLancamentoOperacaoInvalida()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "0102050",
                Operacao = (EOperacao)1234,
                IdContaDestino = "0102030",
                IdContaOrigem = "0102040",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Operação não suportada.");
        }

        [Fact]
        public async void TesteCreatelancamentoContaDestNull()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "0102050",
                Operacao = EOperacao.Deposito,
                IdContaDestino = "",
                IdContaOrigem = "0103020",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "IdContaDestino não pode ser nulo.");
        }


        [Fact]
        public async void TesteSaqueContaOrigemNull()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "0102050",
                Operacao = EOperacao.Saque,
                IdContaDestino = "010230",
                IdContaOrigem = "",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "IdContaOrigem não pode ser nulo.");
        }

        [Fact]
        public async void TesteTransferenciaContaOrigemNull()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "0102050",
                Operacao = EOperacao.Transferencia,
                IdContaDestino = "010230",
                IdContaOrigem = "",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "IdContaOrigem não pode ser nulo.");
        }

        [Fact]
        public async void TesteTransferenciaContaDestNull()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "0102050",
                Operacao = EOperacao.Transferencia,
                IdContaDestino = "",
                IdContaOrigem = "010230",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "IdContaDestino não pode ser nulo.");
        }

        [Fact]
        public async void TesteCreatelancamentoValorZero()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "0102050",
                Operacao = EOperacao.Transferencia,
                IdContaDestino = "010220",
                IdContaOrigem = "010230",
                Valor = 0
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Valor precisa ser maior que R$ 0,00.");
        }

        [Fact]
        public async void TesteDepositoContaDestNull()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "0102050",
                Operacao = EOperacao.Deposito,
                IdContaDestino = "",
                IdContaOrigem = "010320",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "IdContaDestino não pode ser nulo.");
        }

        [Fact]
        public async void TesteTransferenciContaDestNull()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "0102050",
                Operacao = EOperacao.Transferencia,
                IdContaDestino = "",
                IdContaOrigem = "010230",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "IdContaDestino não pode ser nulo.");
        }

        [Fact]
        public async void TesteLancamentonaoEncontrado()
        {
            var result = await _lancamentoAppService.GetByIdAsync<LancamentoResponse>("0");
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Lancamento não encontrado.");
            Assert.True(result.Value == null);
            Assert.True(result.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async void Testelancamentonull()
        {
            var result = await _lancamentoAppService.GetByIdAsync<LancamentoResponse>(null);
            Assert.True(result.StatusCode == HttpStatusCode.NotFound);
            Assert.True(result.Value == null);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Lancamento não encontrado.");
        }

        [Fact]
        public async void TestGetLancamentoOk()
        {
            var result = await _lancamentoAppService.GetByIdAsync<LancamentoResponse>("000003");
            Assert.True(result.StatusCode == HttpStatusCode.OK);
            Assert.True(result.Value.Id == "000003");
            Assert.True(result.Errors == null);
        }

        [Fact]
        public async void TesteDeleteNotFound()
        {
            var result = await _lancamentoAppService.DeleteAsync("0", _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.NotFound);
            Assert.True(result.Value == 0);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Lancamento não encontrado.");
        }

        [Fact]
        public async void TesteDeleteIdNull()
        {
            var result = await _lancamentoAppService.DeleteAsync(null, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(result.Value == 0);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Id não pode ser nulo.");
        }

        [Fact]
        public async void TesteDeleteok()
        {
            var result = await _lancamentoAppService.DeleteAsync("000055", _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.OK);
            Assert.True(result.Value == 1);
            Assert.True(result.Errors == null);
        }

        [Fact]
        public async void TesteOperacaoInvalida()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "0102050",
                Operacao = (EOperacao)43434,
                IdContaDestino = "",
                IdContaOrigem = "",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Operação não suportada.");
        }

        [Fact]
        public async void TesteLancamentoNull()
        {
            var result = await _lancamentoAppService.CreateAsync(null, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Lançamento não pode ser nulo.");
        }


        [Fact]
        public async void TesteDepositoContaDestNotFound()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "0102050",
                Operacao = EOperacao.Deposito,
                IdContaDestino = "0",
                IdContaOrigem = "",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.NotFound);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Conta não encontrado.");
        }

        [Fact]
        public async void TestDepositOk()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "000001",
                Operacao = EOperacao.Deposito,
                IdContaDestino = "000100",
                IdContaOrigem = "",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.Created);
            Assert.True(result.Value == "000001");
            Assert.True(result.Errors == null);
        }


        [Fact]
        public async void TesteSaqueContaOrigenCNotFound()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "000001",
                Operacao = EOperacao.Saque,
                IdContaDestino = "",
                IdContaOrigem = "0",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.NotFound);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Conta não encontrado.");
        }

        [Fact]
        public async void TesteSaqueSaldoInsulficiente()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "000001",
                Operacao = EOperacao.Saque,
                IdContaDestino = "",
                IdContaOrigem = "000100",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Saldo insulficiente.");
        }

        [Fact]
        public async void TesteSaqueOk()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "000001",
                Operacao = EOperacao.Saque,
                IdContaDestino = "",
                IdContaOrigem = "000102",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.Created);
            Assert.True(result.Value == "000001");
            Assert.True(result.Errors == null);
        }

        [Fact]
        public async void TesteTransferenciaContaorigemNull()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "000001",
                Operacao = EOperacao.Transferencia,
                IdContaDestino = "",
                IdContaOrigem = "",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "IdContaOrigem não pode ser nulo.");
        }

        [Fact]
        public async void TesteTransferenciaContaDestinoNull()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "000001",
                Operacao = EOperacao.Transferencia,
                IdContaDestino = "",
                IdContaOrigem = "",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "IdContaOrigem não pode ser nulo.");
            Assert.Contains(result.Errors, x => x.ErrorMessage == "IdContaDestino não pode ser nulo.");
        }

        [Fact]
        public async void TesteTransferenciaContaOrigemNotFound()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "000001",
                Operacao = EOperacao.Transferencia,
                IdContaDestino = "1",
                IdContaOrigem = "0",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.NotFound);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Conta não encontrado.");
        }

        [Fact]
        public async void TesteTransferenciaConteDestinoNotFound()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "000001",
                Operacao = EOperacao.Transferencia,
                IdContaDestino = "0",
                IdContaOrigem = "1",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.NotFound);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Conta não encontrado.");
        }

        [Fact]
        public async void TesteTransferecnciSaldoinsulficiente()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "000001",
                Operacao = EOperacao.Transferencia,
                IdContaDestino = "000102",
                IdContaOrigem = "3",
                Valor = 200
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Saldo insulficiente.");
        }

        [Fact]
        public async void TestTransferenciaOk()
        {
            var lancamento = new LancamentoRequest()
            {
                Id = "000001",
                Operacao = EOperacao.Transferencia,
                IdContaDestino = "3",
                IdContaOrigem = "000102",
                Valor = 100
            };
            var result = await _lancamentoAppService.CreateAsync(lancamento, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.Created);
            Assert.True(result.Value == "000001");
            Assert.True(result.Errors == null);
        }

    }
}
