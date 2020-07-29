using System;
using System.Linq;
using Experion.CabO.Services.DTOs;
using Experion.CabO.Services.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Experion.CabO.Controllers
{
    [EnableCors("CabOPolicy")]
    [Route("api/[controller]")]
    [ApiController]

    public class RidesController : Controller
    {
        private IRideService rideService;
        private ILogger<LoginController> logger;

        public RidesController(IRideService rideService, ILogger<LoginController> logger)
        {
            this.rideService = rideService;
            this.logger = logger;
        }
         
        [HttpPost]
        public IActionResult Post([FromBody] EmployeeRideEditDto ride)
        {
            try
            {
                    try
                    {
                        return Ok(rideService.AddRide(ride));
                    }
                    catch
                    {
                        return BadRequest();
                    }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
                throw;
            }
        }
        
        [HttpGet]
        [Route("projects/{id}")]
        public IActionResult GetProjects(int id)
        {
            try
            {
                var result = rideService.ProjectsGet(id);
                if (result == null)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(result.Result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
                throw;
            }
        }
        [HttpPatch]
        [Route("employee/cancel/{id}")]
        public IActionResult Cancel(int id)
        {
            try
            {
                if (rideService.CancelRide(id))
                {
                    return Ok(true);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
                throw;
            }
        }
        

        [HttpPatch]
        [Route("driver-approve/{id}")]
        public IActionResult UpdateRideApproved(int id)
        {
            try
            {
                var response = rideService.ApproveRideByDriver(id);
                if (response)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPatch]
        [Route("approve/{id}")]
        public IActionResult UpdateRideApproved(int id, [FromBody] ApproveRideDto approveRide)
        {
            try
            {
                var result = rideService.ApproveRide(id, Convert.ToInt32(approveRide.CabId), approveRide.CabType, approveRide.ExternalCabName);
                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPatch]
        [Route("complete/{id}")]
        public IActionResult UpdateRideCompleted(int id)
        {
            try
            {
                var result = rideService.CompleteRide(id);
                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPatch]
        [Route ("driver-reject/{id}")]
        public IActionResult UpdateRideRejected(int id)
        {
            try
            {
                var result = rideService.RejectRideByDriver(id);
                if (result)
                {
                    return Json(true);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPatch]
        [Route("reject/{id}")]
        public IActionResult UpdateRideRejected(int id, [FromBody] RejectRideDto rejectRide)
        {
            try
            {
                var result = rideService.RejectRide(id, rejectRide.CancelReason);
                if (result)
                {
                    return Json(true);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("cabs")]
        public IActionResult GetCabList(DateTime rideDate, DateTime rideTime)
        {
            try
            {
                var result = rideService.GetCabList(rideDate, rideTime);
                if (result == null || !result.Any())
                {
                    return NoContent();
                }
                else
                {
                    return Ok(result);
                }
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPatch]
        [Route("checkTime")]
        public IActionResult CheckTime(CheckTimeInfo rideInfo)
        {
            try
            {
                var result = rideService.CheckTime(rideInfo);
                if (result != null)
                {
                    return Ok(result);
                }
                else if (result == null)
                {
                    return NoContent();
                }
            }
            catch(Exception e)
            {
                return Ok(e);
            }
            return null;
        }
        [HttpGet]
        [Route("availableTime")]
        public IActionResult GetAvailableTimes(int location1, int location2)
        {
            try
            {
                var result = rideService.GetAvailableTimes(location1, location2);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NoContent();
                }
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("offices")]
        public IActionResult GetOffices()
        {
            try
            {
                var result = rideService.GetAssignedOffices();
                if (result == null || !result.Any())
                {
                    return NoContent();
                }
                else
                {
                    return Ok(result);
                }
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("correspondingOffices")]
        public IActionResult GetCorrespondingOffices(int officeId)
        {
            try
            {
                var result = rideService.GetCorrespondingOffices(officeId);
                if (result == null || !result.Any())
                {
                    return NoContent();
                }
                else
                {
                    return Ok(result);
                }
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("completedInfo")]
        public IActionResult GetCompletedRideInfo(int id)
        {
            try
            {
                var result = rideService.GetCompletedRideInfo(id);
                if (result == null)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(result);
                }
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("driver/ride-assignment-id/{id}")]
        public IActionResult GetRideAssignment(int id)
        {
            try
            {
                var result = rideService.RideAssignmenId(id);
                if (result == 0)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(result);
                }
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPatch("initial-reading/{id}")]
        public IActionResult updateInitialReading(int id, long reading)
        {
            try
            {
                return (Ok(rideService.updateInitialReading(id, reading)));
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPatch("final-reading/{id}")]
        public IActionResult updateFinalReading(int id, long reading)
        {
            try { 
                return Ok(rideService.updateFinalReading(id, reading));
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPatch]
        [Route("mail")]
        public IActionResult sendMail(string purpose, int id)
        {
            try
            {
                if (purpose == "rideAdded")
                {
                    return Ok(rideService.sendMailRideAdded(id));
                } 
                else
                {
                    return Ok(rideService.sendMail(id, purpose));
                }
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }
        [HttpPatch]
        [Route("cabDriverInfo")]
        public IActionResult getCabDriverInfo(int rideId)
        {
            try
            {
                return Ok(rideService.getCabDriverInfo(rideId));
            }
            catch (Exception e)
            {
                return NoContent();
            }
        }
    }
}