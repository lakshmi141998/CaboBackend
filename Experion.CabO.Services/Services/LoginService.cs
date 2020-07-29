using Experion.CabO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Experion.CabO.Services.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Experion.CabO.Services.Services
{
    public class LoginService : ILoginService
    {
        private CabODbContext cabODbContext;
        private IConfiguration configuration;
        private TtsApiService apiService;
        public LoginService(CabODbContext cabODbContext, IConfiguration configuration,TtsApiService _apiService)
        {
            this.cabODbContext = cabODbContext;
            this.configuration = configuration;
            apiService = _apiService;
        }
        public async Task<TTSUserDetailsByEmail> LoginCheck(string email)
        {
            try
            {
                var user = await apiService.GetTTSUserDetailsByEmail(email);
                if (user == null)
                {
                    return user;
                }
                else if (user.isActive == false)
                {
                    user = null;
                    return user;
                }
                else
                {
                    return user;
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public TokenDto GenerateJWTToken(TTSUserDetailsByEmail user,Payload payload)
        {
            try
            {
                var Role = user.designation;
                var BUName = "NULL";
                var key = configuration.GetValue<string>("SecretKey");
                var symmetricToken = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var SigningCredentials = new SigningCredentials(symmetricToken, SecurityAlgorithms.HmacSha256Signature);
                var fullName = user.firstName + " " + user.lastName;
                var claims = new List<Claim>();
                claims.Add(new Claim("Email", payload.Email));
                claims.Add(new Claim("ImageUrl", payload.Picture));
                claims.Add(new Claim("Name", fullName));
                claims.Add(new Claim("UserId", user.userId.ToString()));
                claims.Add(new Claim("RoleId", Role.id.ToString()));
                claims.Add(new Claim("Role", Role.name));
                claims.Add(new Claim("BusinessUnitId", user.businessUnitId.ToString()));
                claims.Add(new Claim("BusinessUnit", BUName));
                claims.Add(new Claim("PhoneNo", user.mobileNo));
                var tokenDescriptor = new JwtSecurityToken(
                        issuer: "CabOAdmin",
                        audience: "CabOUser",

                        signingCredentials: SigningCredentials,
                        expires: DateTime.Now.AddHours(2),
                        claims: claims
                    );
                var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);


                TokenDto returnToken = new TokenDto();
                returnToken.token = token;
                return returnToken;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public TokenDto DriverCheck(DriverLoginDto driverLogin)
        {
            var driver = cabODbContext.Driver.Where(x => x.UserName == driverLogin.Username && x.Password == driverLogin.Password).FirstOrDefault();
            if(driver ==null)
            {
                return null;
            }
            else
            {
                
                return GenerateDriverJwt(driver);
            }
        }
        public TokenDto GenerateDriverJwt(Driver driver )
        {
            var key = configuration.GetValue<string>("SecretKey");
            var symmetricToken = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var SigningCredentials = new SigningCredentials(symmetricToken, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>();
            claims.Add(new Claim("DriverName", driver.Name));
            claims.Add(new Claim("DriverId", driver.Id.ToString()));
            claims.Add(new Claim("PhoneNo", driver.PhoneNo));

            var tokenDescriptor = new JwtSecurityToken(
                   issuer: "CabOAdmin",
                   audience: "CabOUser",

                   signingCredentials: SigningCredentials,
                   expires: DateTime.Now.AddHours(2),
                   claims: claims
               );
            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            TokenDto returnToken = new TokenDto();
            returnToken.token = token;
            return returnToken;
        }
    }
}
