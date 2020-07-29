using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Experion.CabO.Services.DTOs;
using Experion.CabO.Services.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Experion.CabO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CabOPolicy")]
    public class LoginController : ControllerBase
    {
        private ILoginService loginService;
        
        private ILogger<LoginController> logger;

        public LoginController(ILoginService loginService, ILogger<LoginController> logger)
        {
            this.loginService = loginService;
            this.logger = logger;
        }
        
        [HttpPost]
        public async Task<IActionResult> POSTAsync([FromBody] TokenDto value)
        {
            try
           {
                GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();
                //settings.Audience = new List<string>() { "781444592241-5l8d33p832p4muissesek9gvr74p08rk.apps.googleusercontent.com" };
                settings.Audience = new List<string>() { "491676760446-n2786agrpa456hcr6cfns835t130glnm.apps.googleusercontent.com" };
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(value.token, settings);
                if (payload.EmailVerified)
                {
                    Payload userPayload = new Payload
                    {
                        Name = payload.Name,
                        Email = payload.Email,
                        EmailVerified = payload.EmailVerified,
                        Picture = payload.Picture
                    };
                    TTSUserDetailsByEmail user = loginService.LoginCheck(payload.Email).Result;
                    if (user != null)
                    {
                        var token = loginService.GenerateJWTToken(user, userPayload);
                        return Ok(token);
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
                throw;
            } 
        }
        [HttpPost]
        [Route("driver")]
        public async Task<IActionResult> POSTdriver([FromBody] DriverLoginDto value)
            {
            try
            {
                if(value != null)
                {
                    var token = loginService.DriverCheck(value);
                    if (token == null)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        return Ok(token);
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch( Exception ex)
            {
                return StatusCode(500);
                throw;
            }
        }
    }
}
