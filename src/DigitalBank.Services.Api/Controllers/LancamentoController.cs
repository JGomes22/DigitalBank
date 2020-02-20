using DigitalBank.Application.AppServices.Interfaces;
using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalBank.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "admin")]
    public class LancamentoController : Controller
    {
        private readonly ILancamentoAppService _lancamentoAppService;
        public LancamentoController(ILancamentoAppService lancamentoAppService)
        {
            _lancamentoAppService = lancamentoAppService;
        }

        [HttpPost]
        public async Task<ActionResult> Post(LancamentoRequest lancamentoRequest, CancellationToken cancellationToken)
        {
            var result = await _lancamentoAppService.CreateAsync(lancamentoRequest, cancellationToken);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("{contaId}")]
        public async Task<ActionResult> Get(string contaId)
        {
            var result = await _lancamentoAppService.GetAsync<LancamentoResponse>(x => x.IdContaDestino == contaId || x.IdContaOrigem == contaId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}