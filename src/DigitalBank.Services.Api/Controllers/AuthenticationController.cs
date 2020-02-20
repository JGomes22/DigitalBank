using DigitalBank.Domain.DTO.Request;
using DigitalBank.Domain.DTO.Response;
using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace DigitalBank.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Post([FromBody]UserRequest userRequest)
        {
            if (userRequest.User != "admin" || userRequest.Password != "password")
                return StatusCode((int)HttpStatusCode.BadRequest, new HttpResult<string>(null, HttpStatusCode.BadRequest, Error.GenerateFailure("Usuário ou senha inválidos.")));

            string token = await _authenticationService.GetJwtToken(userRequest.User);

            var userResponse = new UserResponse
            {
                User = userRequest.User,
                Token = token
            };

            return new HttpResult<UserResponse>(userResponse, HttpStatusCode.OK, null);
        }
    }
}