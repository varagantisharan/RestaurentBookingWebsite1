using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurentBookingWebsite.DbModels;
using RestaurentBookingWebsite.Services;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminServicesAPIController : ControllerBase
    {
        //private LoginService _loginService;
        //public AdminServices _adminService;
        //private readonly ILogger<AdminServicesAPIController> _logger;
        //public AdminServicesAPIController(LoginService loginService, AdminServices adminService, ILogger<AdminServicesAPIController> logger) 
        //{
        //    _loginService = loginService;
        //    _adminService = adminService;
        //    _logger = logger;
        //}

        //[HttpPost]
        //public IActionResult checkOut(CheckIn checkOut)
        //{
        //    if (checkOut.CheckinId < 0)
        //    {
        //        return BadRequest();
        //    }
        //    else
        //    {
        //        int grossAmount = 0;
        //        if (grossAmount != 0)
        //        {
        //            grossAmount = (int)checkOut.GrossAmount;
        //            int res = _adminService.UpdateCheckOutDetails(checkOut.CheckinId, grossAmount);
        //            if (res != 0)
        //            {
        //                return Ok(res);
        //            }
        //            else
        //            {
        //                return BadRequest();
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest("Something Went Wrong");
        //        }

        //    }
        //}

        //[HttpGet("GetCheckIn/{id}")]
        //public async Task<ActionResult<Admin>> GetCheckIn(int id)
        //{
        //    var res = _adminService.GetCheckIn(id);

        //    if (res == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(res);
        //}

    }
}
