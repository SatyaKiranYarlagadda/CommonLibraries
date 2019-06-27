using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace service_api.Api.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TestController : ControllerBase
    {

        [HttpGet("medical"), Authorize("MedicalUser", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[HttpGet("medical"), Authorize("User", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetMedical()
        {
            return Ok(new { Status = "Successfully returned from medical" });
        }

        [HttpGet("hospital"), Authorize("HospitalUser", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetHospital()
        {
            return Ok(new { Status = "Successfully returned from hospital" });
        }

        [HttpGet("ancillary"), Authorize("AncillaryUser", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetAncillary()
        {
            return Ok(new { Status = "Successfully returned from ancillary" });
        }
    }
}
