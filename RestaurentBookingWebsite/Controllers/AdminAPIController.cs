using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurentBookingWebsite.DbModels;
using RestaurentBookingWebsite.Services;
using System.Text.Json;
using System.Text.Json.Nodes;
using Entity_Layer;
using Humanizer;
using RestaurentBookingWebsite.Models;

namespace RestaurentBookingWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAPIController : ControllerBase
    {
        private IAdmin _adminService;
        public AdminAPIController(IAdmin adminService)
        {
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

        [HttpGet]
        [Route("GetAllAdminDetails")]
        public async Task<ActionResult<Admin>> GetAllAdminDetails()
        {
            var res = _adminService.GetAllAdminDetails();

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        [HttpPost]
        [Route("DateRangeBookings")]
        public async Task<IActionResult> DateRangeBookings([FromBody] Dictionary<string, string> data)
        {
            var From = Convert.ToDateTime(data["FromDate"] +"/"+ data["FromMonth"] +"/"+ data["FromYear"]);
            var To = Convert.ToDateTime(data["ToDate"] + "/" + data["ToMonth"] + "/" + data["ToYear"]);
            var bookings = _adminService.BookingsAsPerDateRange(From, To);
            if(bookings == null)
            {
                return BadRequest("No bookings found");
            }
            return Ok(bookings);    
        }

        [HttpGet]
        [Route("GetCustomerBookingDetails")]
        public async Task<IActionResult> GetCustomerBookingDetails()
        {
            List<Booking> bookings =  _adminService.UpcomingThreeDaysBookings();
            List<Customer> customers = _adminService.GetBookedCustomerDetails();
            List<CheckIn> checkins = _adminService.GetAllCheckIns();

            var customerBookings = from c in customers
                                   join b in bookings on c.CustomerId equals b.CustomerId into table1
                                   from b in table1
                                   join ch in checkins on b.BookingId equals ch.BookingId into table2
                                   from ch in table2.DefaultIfEmpty().ToList()
                                   select new CustomerBookingModel
                                   {
                                       Customer = c,
                                       Booking = b,
                                       CheckIn = ch,
                                   };
            return Ok(customerBookings);
        }


        [HttpPost]
        [Route("GetCustomerBookingByUserId")]
        public async Task<IActionResult> GetCustomerBookingByUserId([FromBody] String UserId)
        {
            List<Booking> bookings = _adminService.UpcomingThreeDaysBookings();
            List<Customer> customers = _adminService.GetBookedCustomerDetails().Where(c => c.UserId == UserId).ToList();
            List<CheckIn> checkins = _adminService.GetAllCheckIns();

            var customerBookings = from c in customers
                                   join b in bookings on c.CustomerId equals b.CustomerId into table1
                                   from b in table1
                                   join ch in checkins on b.BookingId equals ch.BookingId into table2
                                   from ch in table2.DefaultIfEmpty().ToList()
                                   select new CustomerBookingModel
                                   {
                                       Customer = c,
                                       Booking = b,
                                       CheckIn = ch,
                                   };            
            return Ok(customerBookings);
        }

    }
}
