using Entity_Layer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurentBookingWebsite.DbModels;
using RestaurentBookingWebsite.Services;


namespace RestaurentBookingWebsite.Controllers
{
    [Authorize]
    public class CustomerDashboardController : Controller
    {       
        public IActionResult CustDashboard(int id)
        {
            TempData["UserName"] = HttpContext.Session.GetString("CustomerName");
            ViewBag.CustomerId = HttpContext.Session.GetInt32("CustomerId");
            DateTime current_day = DateTime.Now;
            TempData["minDate"] = current_day.ToString("yyyy-MM-dd");
            TempData["maxDate"] = current_day.AddDays(2).ToString("yyyy-MM-dd");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(BookingsModel model, int id)
        {
            model.customer_id = id ;
            ViewBag.CustomerId = HttpContext.Session.GetInt32("CustomerId");
            model.date = model.booking_date.Day;
            model.month = model.booking_date.Month;
            Dictionary<string,string> data = new Dictionary<string,string>();
            data["date"] = model.date.ToString();
            data["month"] = model.month.ToString();
            data["customer_id"]=model.customer_id.ToString();
            data["slot_time"] = model.slot_Time;

            List<Admin> adminDetailsLst = new List<Admin>();
            Booking bookingDetails = new Booking();
            Customer customerDetails = new Customer();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5048/api/");

                var Res = client.PostAsJsonAsync("CustomerAPI/NewBooking/", data);
                Res.Wait();

                var Result = Res.Result;
                if (Result.IsSuccessStatusCode)
                {
                    var res = Result.Content.ReadAsAsync<Booking>().Result;
                    if (res!=null)
                    {
                        HttpResponseMessage Res2 = await client.GetAsync("CustomerAPI/GetBookingsById/" + res.BookingId.ToString());
                        HttpResponseMessage Res3 = await client.GetAsync("AdminAPI/GetAllAdminDetails/");
                        HttpResponseMessage Res4 = await client.GetAsync("CustomerAPI/GetCustomerById/" + model.customer_id.ToString());

                        if (Res2.IsSuccessStatusCode)
                        {
                            var Resp = Res2.Content.ReadAsAsync<Booking>();
                            Resp.Wait();

                            bookingDetails = Resp.Result;                                                      
                        }
                        if (Res3.IsSuccessStatusCode)
                        {
                            var Resp = Res3.Content.ReadAsAsync<List<Admin>>();
                            Resp.Wait();

                            adminDetailsLst = Resp.Result;
                        }
                        if (Res4.IsSuccessStatusCode)
                        {
                            var Respn = Res4.Content.ReadAsAsync<Customer>();
                            Respn.Wait();
                            customerDetails = Respn.Result;
                        }
                        if (customerDetails != null && bookingDetails != null)
                        {
                            MailRequest mail = new MailRequest();

                            mail.Body = "Dear " + customerDetails.FirstName + " " + customerDetails.LastName + " .<br>" +
                            "Your booking has been completed successfully.<br>" +
                            "Booking Id :" + bookingDetails.BookingId +
                            "<br>Slot Date and time :" + bookingDetails.BookingDate +
                            "<br>Thank You." +
                            "<br>Best regards," +
                            "<br>Sharan";
                            mail.Subject = "Booking has been confirmed";
                            mail.ToEmail = customerDetails.Email;
                            var resp = client.PostAsJsonAsync("LoginAPI/SendEmail/", mail);
                            resp.Wait();
                            var respResult = resp.Result;
                        }
                        if (adminDetailsLst != null)
                        {
                            foreach (var admin in adminDetailsLst)
                            {
                                MailRequest mail = new MailRequest();
                                string message = "Dear Admin " + admin.FirstName + " " + admin.LastName +
                                                 ",<br>A new booking has been confirmed by the Customer: " + customerDetails.FirstName + " " + customerDetails.LastName +
                                                 " with the booking Id: " + bookingDetails.BookingId +
                                                 "<br>Thank You.";

                                string subject = "Booking has been confirmed";

                                mail.Body = message;
                                mail.Subject = subject;
                                mail.ToEmail = admin.Email;

                                var resp = client.PostAsJsonAsync("LoginAPI/SendEmail/", mail);
                                Res.Wait();

                                var respResult = resp.Result;

                            }
                        }
                        return RedirectToAction("CustDashboard", "CustomerDashboard", new { @id = model.customer_id });
                    }
                    else
                    {
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
            ViewBag.CustomerId = HttpContext.Session.GetInt32("CustomerId");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5048/api/");
                HttpResponseMessage Res = await client.GetAsync("CustomerAPI/GetBookings/" + id.ToString());
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsAsync<List<Booking>>();
                    Response.Wait();

                    var bookingDetails = Response.Result;
                    if (bookingDetails != null)
                    {
                        return View(bookingDetails);
                    }                    
                }
                ModelState.AddModelError(string.Empty, "Could not found CheckIn Id");
                return View(null);
            }
            
        }

        public async Task<IActionResult> CancelBooking(int id) 
        {
            ViewBag.CustomerId = HttpContext.Session.GetInt32("CustomerId");
            List<Admin> adminDetailsLst = new List<Admin>();
            Booking bookingDetails =  new Booking();
            Customer customerDetails = new Customer();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5048/api/");
                HttpResponseMessage Res = await client.DeleteAsync("CustomerAPI/CancelBooking/" + id.ToString());
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsAsync<int>();
                    Response.Wait();

                    var res = Response.Result;
                    if(res == 1)
                    {
                        HttpResponseMessage Res2 = await client.GetAsync("CustomerAPI/GetBookingsById/" + id.ToString());
                        HttpResponseMessage Res3 = await client.GetAsync("AdminAPI/GetAllAdminDetails/");

                        if (Res2.IsSuccessStatusCode)
                        {
                            var Resp = Res2.Content.ReadAsAsync<Booking>();
                            Resp.Wait();

                            bookingDetails = Resp.Result;
                            HttpResponseMessage Res4 = await client.GetAsync("CustomerAPI/GetCustomerById/" + bookingDetails.CustomerId.ToString());
                            if (Res4.IsSuccessStatusCode)
                            {
                                var Respn = Res4.Content.ReadAsAsync<Customer>();
                                Respn.Wait();
                                customerDetails = Respn.Result;
                            }
                        }
                        if (Res3.IsSuccessStatusCode)
                        {
                            var Resp = Res3.Content.ReadAsAsync<List<Admin>>();
                            Resp.Wait();

                            adminDetailsLst = Resp.Result;
                        }

                        if (customerDetails != null && bookingDetails != null)
                        {
                            MailRequest mail = new MailRequest();

                            mail.Body = "Dear " + customerDetails.FirstName + " " + customerDetails.LastName + " .<br>" +
                                        "Your booking has been Cancelled successfully.<br>" +
                                         "Booking Id :" + bookingDetails.BookingId +
                                        "<br>Slot Date and time :" + bookingDetails.BookingDate +
                                        "<br>Thank You." +
                                        "<br>Best regards," +
                                        "<br>Sharan";
                            mail.Subject = "Cancellation has been confirmed"; 
                            mail.ToEmail = customerDetails.Email;

                            var resp = client.PostAsJsonAsync("LoginAPI/SendEmail/", mail);
                            resp.Wait();

                            var respResult = resp.Result;

                        }
                        if (adminDetailsLst != null)
                        {
                            foreach (var admin in adminDetailsLst)
                            {
                                MailRequest mail = new MailRequest();
                                mail.Body = "Dear Admin " + admin.FirstName + " " + admin.LastName +
                                                                 ",<br>A new Cancelletion has been confirmed by the Customer: " + customerDetails.FirstName + " " + customerDetails.LastName +
                                                                 " with the booking Id: " + bookingDetails.BookingId +
                                                                 "<br>Slot Date and time :" + bookingDetails.BookingDate +
                                                                 "<br>Thank You.";

                                mail.Subject = "Cancellation has been confirmed";                               
                                mail.ToEmail = admin.Email;
                                var resp = client.PostAsJsonAsync("LoginAPI/SendEmail/", mail);
                                resp.Wait();
                                var respResult = resp.Result;
                            }
                        }
                    }                
                }               
                return RedirectToAction("MyBookings", "CustomerDashboard", new { @id = HttpContext.Session.GetInt32("CustomerId") });
            }
        }

    }
}
