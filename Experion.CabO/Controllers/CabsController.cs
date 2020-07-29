using System;
using Microsoft.AspNetCore.Mvc;
using Experion.CabO.Services.Services;
using Experion.CabO.Services.DTOs;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;

namespace Experion.CabO.Controllers
{
    [Route("api/[controller]")]

    [EnableCors("CabOPolicy")]

    [ApiController]

    public class CabsController : ControllerBase
    {
        public ICabService cabService;
        private ILogger<CabsController> logger;

        public CabsController(ICabService cabService, ILogger<CabsController> logger)
        {
            this.cabService = cabService;
            this.logger = logger;

        }

        [HttpGet]

        [Route("get")]
        public IActionResult Index()
        {
            try
            {
                var result = cabService.GetCabs();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]

        [Route("{id}")]
        public IActionResult Cabdto(int id)
        {
            try
            {
                var result = cabService.GetCabById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpPost]
        public IActionResult Create([FromBody] cabdto cabdetails)
        {
            try
            {
                return Ok(cabService.AddCab(cabdetails));
            }
            catch(Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Edit(int id, [FromBody] cabdto cabdetails)
        {
            try
            {
                return Ok(cabService.UpdateCab(id, cabdetails));
            }
            catch(Exception e)
            {
                return Ok(e);
            }
        }

        [HttpDelete]
        [Route ("{id}")]

        public IActionResult Delete(int id)
        {
            try
            {
                if (id != 0)
                {
                    var a = cabService.DeleteCab(id);
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
