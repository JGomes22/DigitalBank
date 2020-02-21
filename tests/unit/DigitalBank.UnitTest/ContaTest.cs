using DigitalBank.Application.AppServices;
using DigitalBank.Application.AppServices.Interfaces;
using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.DTO.Response;
using DigitalBank.Domain.Entities;
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
    public class ContaTest
    {
        private readonly IContaAppService _contaAppService;
        private readonly IContaService _contaService;
        private readonly IContaRepository _contaRepository;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public ContaTest()
        {
            _contaRepository = Substitute.For<IContaRepository>();
            _contaService = new ContaService(_contaRepository);
            _contaAppService = new ContaAppService(_contaService);
            Setup();
        }

        private void Setup()
        {

            _contaRepository.InsertAsync(null, Arg.Any<CancellationToken>()).Returns(Task.FromResult(string.Empty));
            _contaRepository.InsertAsync(Arg.Is<Conta>(x => x.Id == "1"), Arg.Any<CancellationToken>()).Returns(Task.FromResult(string.Empty));
            _contaRepository.InsertAsync(Arg.Is<Conta>(x => x.Id == "123"), Arg.Any<CancellationToken>()).Returns(Task.FromResult("1"));

            _contaRepository.GetByIdAsync("00").Returns(Task.FromResult(default(Conta)));
            _contaRepository.GetByIdAsync("11").Returns(Task.FromResult(new Conta() { Id = "11", Numero = "2131231" }));

            _contaRepository.UpdateAsync(Arg.Any<Expression<Func<Conta, bool>>>(), Arg.Is<Conta>(x => x.Id == "1"), Arg.Any<ReplaceOptions>()).Returns(Task.FromResult(""));
            _contaRepository.UpdateAsync(Arg.Any<Expression<Func<Conta, bool>>>(), Arg.Is<Conta>(x => x.Id == "123"), Arg.Any<ReplaceOptions>()).Returns(Task.FromResult("1"));

            _contaRepository.DeleteAsync("123", Arg.Any<CancellationToken>()).Returns(Task.FromResult(long.Parse("1")));
            _contaRepository.DeleteAsync("11", Arg.Any<CancellationToken>()).Returns(Task.FromResult(long.Parse("0")));

        }

        [Fact]
        public async void TestCreateContaNull()
        {
            var result = await _contaAppService.CreateAsync<ContaRequest>(null, _cancellationToken);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Objeto não pode ser nulo.");
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
        }

        [Fact]
        public async void TestCreateContaOk()
        {
            var conta = new ContaRequest()
            {
                Id = "123",
                Numero = "02312313"
            };
            var result = await _contaAppService.CreateAsync(conta, _cancellationToken);
            Assert.True(result.Errors == null);
            Assert.True(result.StatusCode == HttpStatusCode.Created);
            Assert.True(result.Value == "1");
        }

        [Fact]
        public async void TestCreateContaNumeroVazio()
        {
            var conta = new ContaRequest()
            {
                Id = "123",
                Numero = ""
            };
            var result = await _contaAppService.CreateAsync(conta, _cancellationToken);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Numero não pode ser nulo.");
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
        }

        [Fact]
        public async void TestGetContaNull()
        {
            var result = await _contaAppService.GetByIdAsync<ContaResponse>(null);
            Assert.True(result.StatusCode == HttpStatusCode.NotFound);
            Assert.True(result.Value == null);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Conta não encontrado.");
        }

        [Fact]
        public async void TestGetContaNotFound()
        {
            var result = await _contaAppService.GetByIdAsync<ContaResponse>("0");
            Assert.True(result.Value == null);
            Assert.True(result.StatusCode == HttpStatusCode.NotFound);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Conta não encontrado.");

        }

        [Fact]
        public async void TestGetContaOk()
        {
            var result = await _contaAppService.GetByIdAsync<ContaResponse>("11");
            Assert.True(result.StatusCode == HttpStatusCode.OK);
            Assert.True(result.Value.Id == "11");
            Assert.True(result.Errors == null);
        }

        [Fact]
        public async void TestUpdateContaNotFound()
        {
            var conta = new ContaRequest
            {
                Id = "0",
                Numero = "0102030-10"
            };
            var result = await _contaAppService.UpdateAsync(x => x.Id == "0", conta, null);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Conta não encontrado.");
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
        }

        [Fact]
        public async void TestUpdateContaNullError()
        {
            var result = await _contaAppService.UpdateAsync<ContaRequest>(x => x.Id == "ABC", null, null);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Objeto é nulo.");
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
        }

        [Fact]
        public async void TestUpdateContaNumeroVazioError()
        {
            var conta = new ContaRequest()
            {
                Id = "ABC",
                Numero = ""
            };
            var result = await _contaAppService.UpdateAsync(x => x.Id == "ABC", conta, null);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Numero não pode ser nulo.");
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(string.IsNullOrWhiteSpace(result.Value));
        }


        [Fact]
        public async void TestUpdateContaFilterNullError()
        {
            var conta = new ContaRequest()
            {
                Id = "ABC"
            };

            var result = await _contaAppService.UpdateAsync(null, conta, null);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Filtro não pode ser nulo.");
            Assert.True(result.Value == null);
        }


        [Fact]
        public async void TestUpdateContaOk()
        {
            var conta = new ContaRequest()
            {
                Id = "123",
                Numero = "0102030-10"
            };
            var result = await _contaAppService.UpdateAsync(x => x.Id == "123", conta, null);
            Assert.True(result.StatusCode == HttpStatusCode.OK);
            Assert.True(result.Errors == null);
            Assert.True(result.Value == "1");
        }

        [Fact]
        public async void TestDeleteContaNotFound()
        {
            var result = await _contaAppService.DeleteAsync("0", _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.NotFound);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Conta não encontrado.");
            Assert.True(result.Value == 0);
        }

        [Fact]
        public async void TestDeleteContaIdNullError()
        {
            var result = await _contaAppService.DeleteAsync(null, _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(result.Value == 0);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Id não pode ser nulo.");
        }

        [Fact]
        public async void TestDeleteContaOk()
        {
            var result = await _contaAppService.DeleteAsync("123", _cancellationToken);
            Assert.True(result.StatusCode == HttpStatusCode.OK);
            Assert.True(result.Errors == null);
            Assert.True(result.Value == 1);
        }
       
    }
}
