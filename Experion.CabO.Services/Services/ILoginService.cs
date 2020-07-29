using Experion.CabO.Data.Entities;
using Experion.CabO.Services.DTOs;
using System.Threading.Tasks;

namespace Experion.CabO.Services.Services
{
     public interface ILoginService
    {
        Task<TTSUserDetailsByEmail> LoginCheck(string Email);
        TokenDto GenerateJWTToken(TTSUserDetailsByEmail user, Payload payload);
        TokenDto GenerateDriverJwt(Driver driver);
        TokenDto DriverCheck(DriverLoginDto driverLogin);
    }
}
