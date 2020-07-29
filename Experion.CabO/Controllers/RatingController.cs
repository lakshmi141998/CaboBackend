using System;
using Experion.CabO.Services.DTOs;
using Experion.CabO.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Experion.CabO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : Controller
    {
        private IRatingService ratingService;
        private ILogger<LoginController> logger;

        public RatingController(IRatingService ratingService, ILogger<LoginController> logger)
        {
            this.ratingService = ratingService;
            this.logger = logger;
        }

        [HttpGet("{rideGuid}")]
        public IActionResult checkRating(string rideGuid)
        {
            try
            {
                return Ok(ratingService.checkRating(rideGuid));
            }
            catch(Exception e)
            {
                return NoContent();
            }
        }

        [HttpPost]
        public IActionResult addRating([FromBody] RatingInfo rating)
        {
            try
            {
                return Ok(ratingService.addRating(rating));
            }
            catch (Exception e)
            {
                return NoContent();
            }
        }
    }
}
