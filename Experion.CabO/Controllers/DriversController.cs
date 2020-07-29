using System;
using System.Linq;
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
    public class DriversController : ControllerBase
    {

        IDrivers adddriver;
        private ILogger<CabsController> logger;
        public DriversController(IDrivers adddriver, ILogger<CabsController> logger)
        {
            this.adddriver = adddriver;
            this.logger = logger;

        }
        [HttpPost]
        public IActionResult Post([FromBody]DriverDetails add)
        {
            try
            {
                if (add != null)
                {
                    var r = adddriver.PostDriverDetails(add);
                    if (r == "Posted")
                    {
                        return Ok(true);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var driver = adddriver.GetDriverDetails();
                if (driver == null || !driver.Any())
                {
                    return NoContent();
                }
                else
                {
                    return Ok(driver);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            try
            {
                if (id != 0)
                {
                    return Ok(adddriver.GetDriver(id));
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
          [HttpPut("UpdateDriverDetails/{id}")]
          public  IActionResult Put(int id, [FromBody] DriverDetails edit)
          {
            try
            {
                if (id != null && edit != null)
                {
                    var r = adddriver.UpdateDriverDetails(id, edit);
                    if (r == "Updated")
                    {
                        return Ok(true);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
          [HttpPatch("DeleteDriver/{id}")]
          public IActionResult delet(int id)
          {
            try
            {
                if (id != null)
                {
                    return Ok(adddriver.DeleteDriver(id));
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
