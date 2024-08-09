using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurentBookingWebsite.DbModels;
using RestaurentBookingWebsite.Services;
using System.Text.Json;
using System.Text.Json.Nodes;
using Entity_Layer;

namespace RestaurentBookingWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAPIController : ControllerBase
    {
        private ILogin _loginService;
        private IAdmin _adminService;
        public AdminAPIController(ILogin loginService, IAdmin adminService)
        {
            _loginService = loginService;
            _adminService = adminService;
        }

        [HttpPost]
        [Route("CheckIn")]
        public async Task<IActionResult> CheckIn([FromBody] int id)
        {
            try
            {
                int res = _adminService.UpdateCheckInDetails(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CheckOut")]
        public async Task<IActionResult> CheckOut([FromBody] CheckInsModel checkIn)
        {
            if (checkIn.gross_amount == 0 || checkIn.checkin_id <= 0)
            {
                return BadRequest("Invalid amount or checkin Id");
            }           
            int res = _adminService.UpdateCheckOutDetails(checkIn.checkin_id, (int)checkIn.gross_amount,checkIn.booking_id);
            if (res != 0)
            {
                return Ok(res);
            }
            return BadRequest("Could not update checkout details");
        }


        [HttpGet("GetCheckIn/{id}")]
        public async Task<ActionResult<Admin>> GetCheckIn(int id)
        {
            var res = _adminService.GetCheckIn(id);

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        [HttpGet]
        [Route("UpcomingThreeDaysBookings")]
        public async Task<ActionResult<Admin>> UpcomingThreeDaysBookings()
        {
            var res = _adminService.UpcomingThreeDaysBookings();

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        [HttpGet]
        [Route("CustRegisteredInSevenDays")]
        public async Task<ActionResult<Admin>> CustRegisteredInSevenDays()
        {
            var res = _adminService.CustRegisteredInSevenDays();

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        [HttpGet]
        [Route("CancellationForNextThreedays")]
        public async Task<ActionResult<Admin>> CancellationForNextThreedays()
        {
            var res = _adminService.CancellationForNextThreedays();

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }
    }
}
