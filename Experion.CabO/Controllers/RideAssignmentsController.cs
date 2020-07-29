using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Experion.CabO.Data.Entities;
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
    public class RideAssignmentsController : Controller
    {
        private IRideAssignmentService _rideAssignment;
        private ILogger<RideAssignmentsController> logger;

        public RideAssignmentsController(IRideAssignmentService rideAssignment, ILogger<RideAssignmentsController> logger)
        {
            this._rideAssignment = rideAssignment;
            this.logger = logger;
        }

        [HttpGet]
        [Route("view")]
        public IActionResult GetRideAssignment()
        {
            try
            {
                var ride = _rideAssignment.ViewAssignment();
                if (ride == null || !ride.Any())
                {
                    return NoContent();
                }
                return Ok(ride);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Post([FromBody]RideAssignmentDto ride)
        {
            try
            {
                if (ride == null)
                {
                return BadRequest();
                }
                else
                {
                  var res = _rideAssignment.AddRide(ride);
                    if(res == 0)
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
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("update/{id}")]
        public IActionResult Put(int id, [FromBody]RideAssignmentDto updateRide)
        {
            try
            {
                if (id != 0)
                {
                    var a = _rideAssignment.UpdateRide(id, updateRide);
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

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (id != 0)
                {
                    var a = _rideAssignment.DeleteRide(id);
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

        [HttpGet]
        [Route("cabs")]
        public IActionResult GetCabName()
        {
            try
            {
                var cab = _rideAssignment.GetCabs();
                if (cab == null || !cab.Any())
                {
                    return NoContent();
                }
                return Ok(cab);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("drivers")]
        public IActionResult GetDriverName()
        {
            try
            {
                var driver = _rideAssignment.GetDrivers();
                if (driver == null || !driver.Any())
                {
                    return NoContent();
                }
                return Ok(driver);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("shifts")]
        public IActionResult GetShiftName()
        {
            try
            {
                var shift = _rideAssignment.GetShifts();
                if (shift == null || !shift.Any())
                {
                    return NoContent();
                }
                return Ok(shift);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
