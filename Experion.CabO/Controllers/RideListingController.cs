using Experion.CabO.Services.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Experion.CabO.Controllers
{
    [Route("api/[controller]")]

    [EnableCors("CabOPolicy")]

    [ApiController]
    public class RideListingController : ControllerBase
    {
        public IRideListingService rideListingService;
        private ILogger<RideListingController> logger;

        public RideListingController(IRideListingService rideListingService, ILogger<RideListingController> logger)
        {
            this.rideListingService = rideListingService;
            this.logger = logger;
        }

        [HttpGet("options")]
        public IActionResult Index()
        {
            try
            {
                var result = rideListingService.GetOptions();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet("latest-ride")]
        public IActionResult Index2()
        {
            try
            {
                var result = rideListingService.GetLatestRide();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("date")]
        public IActionResult getRidesDateWise(DateTime start_date, DateTime end_date, string status)
        {
            try
            {
                var result = rideListingService.GetRidesDateWise(start_date, end_date, status);
                if (result == null)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult getRides(string status)
        {
            try
            {
                if (status == null)
                {
                    return NoContent();
                }
                else
                {
                    var result = rideListingService.GetRides(status);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("userDetail/{id}")]

        public IActionResult GetUserById(int id)
        {
            try
            {
                var result = rideListingService.ViewUserDetails(id);
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
                return BadRequest();
            }
        }
    }
}
