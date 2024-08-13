using DocumentFormat.OpenXml.Office2010.Excel;
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
        private IBooking _bookingsServices;

        public CustomerAPIController(IBooking bookingServices)
        {
            _bookingsServices = bookingServices;
        }

        //This method is to get the bookings for the particular customer id
        [HttpGet]
        [Route("GetBookings/{id}")]
        public async Task<ActionResult> GetBookings(int id) //id=customer id
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
        public async Task<IActionResult> NewBooking([FromBody] Dictionary<string,string> data)
        {
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


        [HttpGet]
        [Route("GetBookingsById/{id}")]
        public async Task<ActionResult> GetBookingsById(int id)
        {
            var res = _bookingsServices.GetBookingDetails(id);

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        [HttpGet]
        [Route("GetCustomerById/{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customerDetails = _bookingsServices.GetCustomerDetails(id);
            if (customerDetails!=null)
            {
                return Ok(customerDetails);
            }
            return BadRequest("No customer found with the given Id");
        }

    }
}
