using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.Excel;
using Entity_Layer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurentBookingWebsite.DbModels;
using RestaurentBookingWebsite.Services;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RestaurentBookingWebsite.Controllers

{
    [Authorize]
    public class CustomerDashboardController : Controller
    {
        private LoginService _loginService;
        private BookingServices _bookingsServices;
        private readonly IMail mailService;
       

        public CustomerDashboardController(LoginService loginService,BookingServices bookingServices, IConfiguration configuration, IMail mailService)
        {
            _loginService = loginService;
            _bookingsServices = bookingServices;
            this.mailService = mailService;
        }
        
        public IActionResult CustDashboard(int id)
        {
            TempData["CustId"] = id;
            int custid = Convert.ToInt32(TempData["CustId"]);
            int cid = Convert.ToInt32(TempData["CustomerId"]);
            string Name = _loginService.GetUserName(id, "Customer");
            TempData["UserName"] = Name;
            DateTime current_day = DateTime.Now;
            TempData["minDate"] = current_day.ToString("yyyy-MM-dd");
            //TempData["minDate"] = current_day.AddDays(1).ToString("yyyy-MM-dd");
            TempData["maxDate"] = current_day.AddDays(2).ToString("yyyy-MM-dd");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register(BookingsModel model, int id)
        {
            //int custid = Convert.ToInt32(TempData["CustId"]);
            //int cid = Convert.ToInt32(TempData["CustomerId"]);
            model.customer_id = id ;
            //var res= _bookingsServices.Register(model);
            var adminDetails = _bookingsServices.GetAllAdminDetails();

            model.date = model.booking_date.Day;
            model.month = model.booking_date.Month;
            Dictionary<string,string> data = new Dictionary<string,string>();
            data["date"] = model.date.ToString();
            data["month"] = model.month.ToString();
            data["customer_id"]=model.customer_id.ToString();
            data["slot_time"] = model.slot_Time;
            TempData["CustId"] = model.customer_id;
            TempData["CustomerId"] = model.customer_id;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5093/api/");

                var Res = client.PostAsJsonAsync("CustomerAPI/NewBooking/", data);
                Res.Wait();

                var Result = Res.Result;
                if (Result.IsSuccessStatusCode)
                {
                    var res = Result.Content.ReadAsAsync<int>().Result;
                    if (res == 1 )
                    {
                        var IsValidCustomer = _bookingsServices.GetCustomerDetails(model.customer_id);
                        var IsValidBooking = _bookingsServices.GetBookingDetails(model.customer_id);

                        if (IsValidCustomer != null && IsValidBooking != null)
                        {
                            MailRequest mail = new MailRequest();

                            string message = "Dear " + IsValidCustomer.FirstName + " " + IsValidCustomer.LastName + " .<br>" +
                            "Your booking has been completed successfully.<br>" +
                            "Booking Id :" + IsValidBooking.BookingId +
                            "<br>Slot Date and time :" + IsValidBooking.BookingDate +
                            "<br>Thank You." +
                            "<br>Best regards," +
                            "<br>Sharan";
                            string subject = "Booking has been confirmed";

                            mail.Body = message;
                            mail.Subject = subject;
                            mail.ToEmail = IsValidCustomer.Email;

                            var resp = client.PostAsJsonAsync("LoginAPI/SendEmail/", mail);
                            Res.Wait();

                            var respResult = resp.Result;
                            //  mailService.SendEmail(mail);

                        }
                        if (adminDetails != null)
                        {
                            foreach (var admin in adminDetails)
                            {
                                MailRequest mail = new MailRequest();
                                string message = "Dear Admin " + admin.FirstName + " " + admin.LastName +
                                                 ",<br>A new booking has been confirmed by the Customer: " + IsValidCustomer.FirstName + " " + IsValidCustomer.LastName +
                                                 " with the booking Id: " + IsValidBooking.BookingId +
                                                 "<br>Thank You.";

                                string subject = "Booking has been confirmed";

                                mail.Body = message;
                                mail.Subject = subject;
                                mail.ToEmail = admin.Email;



                                var resp = client.PostAsJsonAsync("LoginAPI/SendEmail/", mail);
                                Res.Wait();

                                var respResult = resp.Result;
                               

                                //  mailService.SendEmail(mail);
                            }
                        }
                        return RedirectToAction("CustDashboard", "CustomerDashboard", new { @id = model.customer_id });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Could not update booking details");
                        throw new Exception("New Booking is not successful");
                    }
                }               
                else
                {
                    throw new Exception("New Booking is not successful");
                }
            }
        }

       // [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyBookings(int id)
        {
            //var bookingDetails = _bookingsServices.GetCustomerBookingDetails(id);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5093/api/");
                HttpResponseMessage Res = await client.GetAsync("CustomerAPI/GetBookings/" + id.ToString());
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsAsync<List<Booking>>();
                    Response.Wait();

                    var bookingDetails = Response.Result;
                    if (bookingDetails != null)
                    {
                        TempData["CustId"] = id;
                        TempData["CustomerId"] = id;
                        return View(bookingDetails);
                    }                    
                }
                ModelState.AddModelError(string.Empty, "Could not found CheckIn Id");
                return View(null);
            }
            
        }

        public async Task<IActionResult> CancelBooking( int id) 
        {
           

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5093/api/");
                HttpResponseMessage Res = await client.DeleteAsync("CustomerAPI/CancelBooking/" + id.ToString());
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsAsync<int>();
                    Response.Wait();

                    if (Response.Result == 1)
                    {
                        return RedirectToAction("MyBookings", "CustomerDashboard", new { @id = Convert.ToInt32(TempData["CustomerId"]) });
                    }

                }
                ModelState.AddModelError(string.Empty, "Could not cancel booking");
                return RedirectToAction("MyBookings", "CustomerDashboard", new { @id = Convert.ToInt32(TempData["CustomerId"]) });
            }
        }

    }
}
