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
    public class OfficeCommutationController : ControllerBase
    {
        private IOfficeCommutationService _officeCommutation;
        private ILogger<OfficeCommutationController> logger;

        public OfficeCommutationController(IOfficeCommutationService officeCommutation, ILogger<OfficeCommutationController> logger)
        {
            _officeCommutation = officeCommutation;
            this.logger = logger;

        }

        [HttpPost]
        [Route("addCommutation")]
        public IActionResult Post([FromBody]OfficeCommutationDto ride)
        {
            try
            {
                if (ride == null)
                {
                    return BadRequest();
                }
                else
                {
                    var res = _officeCommutation.AddOfficeCommutation(ride);
                    if (res == 0)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        return Ok(res);

                    }

                }
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, ex.Message);
                return BadRequest();
            }

        }

        [HttpGet]
        [Route("view")]
        public IActionResult GetOfficeAssignment()
        {
            try
            {
                var ride = _officeCommutation.ViewOfficeCommutation();
                if (ride == null || !ride.Any())
                {
                    return NoContent();
                }
                return Ok(ride);
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("update/{id}")]
        public IActionResult Put(int id, [FromBody]OfficeCommutationDto update)
        {
            try
            {
                if (id != 0)
                {
                    var a = _officeCommutation.UpdateCommutation(id, update);
                    return Ok(a);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, ex.Message);
                return BadRequest();
            }
            
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (id != 0)
                {
                    var a = _officeCommutation.DeleteCommutation(id);
                    return Ok(a);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("addTimings")]
        public IActionResult Post([FromBody]AvailableTimeDto time)
        {
            try
            {
                if (time == null)
                {
                    return BadRequest();
                }
                else
                {
                    var res = _officeCommutation.AddTimings(time);
                    if (res == 0)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        return Ok(res);

                    }
                }
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, ex.Message);
                return BadRequest();
            }
            
        }
        [HttpGet]
        [Route("viewTimings/{id}")]
        public IActionResult GetOfficeTimings(int id)
        {
            try
            {
                var ride = _officeCommutation.ViewTiming(id);
                if (ride == null || !ride.Any())
                {
                    return NoContent();
                }
                return Ok(ride);
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, ex.Message);
                return BadRequest();
            }

        }


        [HttpDelete]
        [Route("deleteTiming/{id}")]
        public IActionResult DeleteTime(int id)
        {
            try
            {
                if (id != 0)
                {
                    var a = _officeCommutation.DeleteTiming(id);
                    return Ok(a);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, ex.Message);
                return BadRequest();
            }
            
        }
    }
}