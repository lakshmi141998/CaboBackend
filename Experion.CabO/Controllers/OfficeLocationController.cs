using System;
using Experion.CabO.Services.DTOs;
using Experion.CabO.Services.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Experion.CabO.Controllers
{
    [Route("api/[controller]")]

    [EnableCors("CabOPolicy")]

    [ApiController]

    public class OfficeLocationController : ControllerBase
    {

        public IOfficeLocationService locationService;
        private ILogger<OfficeLocationController> logger;

        public OfficeLocationController(IOfficeLocationService olocationService, ILogger<OfficeLocationController> logger)
        {
            locationService = olocationService;
            this.logger = logger;
        }

        [HttpPost]
        public IActionResult Post ([FromBody] OfficeLocationDto officelocation)
        {
            try
            {
                if (officelocation == null)
                {
                    return BadRequest();
                }
                var response = locationService.AddLocation(officelocation);
                if (response == 0)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(officelocation);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]

        public IActionResult Index()
        {
            try
            {
                var result = locationService.GetLocations();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete]

        [Route("{id}")]

        public IActionResult DeleteLocation(int id)
        {
            try
            {
                if (id != 0)
                {
                    var a = locationService.DeleteLocation(id);
                    return Ok(a);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}