using DigitalBank.Application.AppServices.Interfaces;
using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalBank.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "admin")]
    public class ContaController : ControllerBase
    {
        private readonly IContaAppService _contaAppService;
        public ContaController(IContaAppService contaAppService)
        {
            _contaAppService = contaAppService;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ContaRequest contaRequest, CancellationToken cancellationToken)
        {
            var result = await _contaAppService.CreateAsync<ContaRequest>(contaRequest, cancellationToken);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, [FromBody] ContaRequest contaRequest, CancellationToken cancellationToken)
        {
            contaRequest.Id = id;
            var result = await _contaAppService.UpdateAsync<ContaRequest>(x => x.Id == id, contaRequest, null);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            var result = await _contaAppService.DeleteAsync(id, cancellationToken);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _contaAppService.GetAsync<ContaResponse>(x => true);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            var result = await _contaAppService.GetByIdAsync<ContaResponse>(id);
            return StatusCode((int)result.StatusCode, result);
        }


    }
}