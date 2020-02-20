using System.Threading.Tasks;

namespace DigitalBank.Domain.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<string> GetJwtToken(string user);
    }
}
