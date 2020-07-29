using System;
using Experion.CabO.Services.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Experion.CabO.Controllers
{
    [EnableCors("CabOPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : Controller
    {
        private IReportService reportService;
        private ILogger<LoginController> logger;

        public ReportsController(IReportService reportService, ILogger<LoginController> logger)
        {
            this.reportService = reportService;
            this.logger = logger;
        }

        [HttpGet] 
       [Route("cabs")]
       public IActionResult GetCabs()
        {
            try
            {
                return Ok(reportService.GetCabList());
            }
            catch (Exception e)
            {
                return NoContent();
            }
        }
        [HttpGet]
        [Route("drivers")]
        public IActionResult GetDrivers()
        {
            try
            {
                return Ok(reportService.GetDriverList());
            }
            catch (Exception e)
            {
                return NoContent();
            }
        }
        [HttpGet]
        [Route("projects")]
        public IActionResult GetProjects()
        {
            try
            {
                return Ok(reportService.GetProjectsList());
            }
            catch (Exception e)
            {
                return NoContent();
            }
        }
        [HttpGet]
        [Route("cabReport")]
        public IActionResult GetCabReport(int cabId, DateTime start, DateTime end)
        {
            try
            {
                return Ok(reportService.GetCabReportStatic(cabId, start, end));
            }
            catch (Exception e)
            {
                return NoContent();
            }
        }
        [HttpGet]
        [Route("rideTypeReport")]
        public IActionResult GetRideReport(string rideType,string rideStatus, DateTime start, DateTime end)
        {
            try
            {
                return Ok(reportService.GetRideReport(rideType,rideStatus, start, end));
            }
            catch (Exception e)
            {
                return NoContent();
            }
        }
        [HttpGet]
        [Route("driverReport")]
        public IActionResult GetDriverReport(int driverId, DateTime start, DateTime end)
        {
            try
            {
                return Ok(reportService.GetDriverReport(driverId, start, end));
            }
            catch (Exception e)
            {
                return NoContent();
            }
        }
        [HttpGet]
        [Route("projectReport")]
        public IActionResult GetProjectReport(string projectCode, DateTime start, DateTime end)
        {
            try
            {
                return Ok(reportService.GetProjectReport(projectCode, start, end));
            }
            catch (Exception e)
            {
                return NoContent();
            }
        }
        [HttpGet]
        [Route("driverRating")]
        public IActionResult GetDriverRating(int driverId) {
            try
            {
                return Ok(reportService.GetDriverRating(driverId));
            }
            catch (Exception e)
            {
                return NoContent();
            }
        }
    }
}
