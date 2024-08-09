using Entity_Layer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurentBookingWebsite.DbModels;
using RestaurentBookingWebsite.Services;

namespace RestaurentBookingWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAPIController : ControllerBase
    {
        private LoginService _loginService;
        private BookingServices _bookingsServices;
        private readonly IMail mailService;

        public CustomerAPIController(LoginService loginService, BookingServices bookingServices, IConfiguration configuration, IMail mailService)
        {
            _loginService = loginService;
            _bookingsServices = bookingServices;
            this.mailService = mailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetCustomer()
        {
            return null;//await Admins.ToListAsync();
        }

        
        [HttpGet]
        [Route("GetBookings/{id}")]
        public async Task<ActionResult> GetBookings(int id)
        {
            var res = _bookingsServices.GetCustomerBookingDetails(id);

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }
        [HttpPost]
        [Route("NewBooking")]
        public async Task<IActionResult> NewBooking([FromBody] Dictionary<string,string> data)//BookingsModel model)
        {
            //string formattedDate = model.booking_date.ToShortDateString() + " " + model.slot_Time;
            //model.booking_date = Convert.ToDateTime(formattedDate);
            BookingsModel bookings = new BookingsModel();
            bookings.month = int.Parse(data["month"]);
            bookings.date = int.Parse(data["date"]);
            bookings.customer_id = int.Parse(data["customer_id"]);
            bookings.slot_Time = data["slot_time"];
            var res = _bookingsServices.Register(bookings);
            if(res == 1)
            {
                return Ok(res);
            }
            return BadRequest("Booking is not successful");
        }

        [HttpDelete]
        [Route("CancelBooking/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var res = _bookingsServices.CancelBooking(id);
            if (res == 1)
            {
                return Ok(res);
            }
            if(res==0)
            {
                return BadRequest("Cancellation is allowed only 24 hrs prior to the booking slot");
            }
            return BadRequest("Booking is not successful");
        }

    }
}
