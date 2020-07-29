using System;
using Experion.CabO.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Experion.CabO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : Controller
    {
        private ILogger<LoginController> logger;
        private IDashboardService dashboardService;

        public DashboardController(IDashboardService dashboardService, ILogger<LoginController> logger)
        {
            this.logger = logger;
            this.dashboardService = dashboardService;
        }

        [HttpGet]
        [Route("employees")]
        public IActionResult GetEmployeeRide(int id)
        {
            try
            {
                return Ok(dashboardService.GetRide(id));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("admin")]
        public IActionResult getRides(DateTime start, string week)
        {
            try
            {
                if (week == "next")
                {
                    start = start.AddDays(1);
                }
                else if (week == "previous")
                {
                    start = start.AddDays(-7);
                }
                DateTime end = start.AddDays(6);
                var result = dashboardService.GetAdminRides(start, end);
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
                return NoContent();
            }
        }
        [HttpGet]
        [Route("ride")]
        public IActionResult GetRideById(string guid)
        {
            try
            {
                var result = dashboardService.GetRideById(guid);
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
                return NoContent();
            }
        }
    }
}
