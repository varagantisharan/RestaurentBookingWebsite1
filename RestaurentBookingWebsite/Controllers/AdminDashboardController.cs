using Microsoft.AspNetCore.Mvc;
using RestaurentBookingWebsite.Services;
using System.Data;
using RestaurentBookingWebsite.DbModels;
using Entity_Layer;
using RestaurentBookingWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Office2010.Excel;


namespace RestaurentBookingWebsite.Controllers
{
    [Authorize]
    public class AdminDashboardController : Controller
    {     
        public async Task<IActionResult> AdmnDashboard()//int id)
        {
            ViewBag.Name = HttpContext.Session.GetString("Username");
            //string Name = _loginService.GetUserName(id, "Admin");

            TempData["UserName"] = ViewBag.Name;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5093/api/");
                HttpResponseMessage Res1 = await client.GetAsync("AdminAPI/UpcomingThreeDaysBookings/");
                HttpResponseMessage Res2 = await client.GetAsync("AdminAPI/CustRegisteredInSevenDays/");
                HttpResponseMessage Res3 = await client.GetAsync("AdminAPI/CancellationForNextThreedays/");
                if (Res1.IsSuccessStatusCode)
                {
                    var Response = Res1.Content.ReadAsAsync<List<Booking>>();
                    Response.Wait();
                    ViewBag.Bookings = Response.Result;
                }
                if (Res2.IsSuccessStatusCode)
                {
                    var Response = Res2.Content.ReadAsAsync<List<Customer>>();
                    Response.Wait();
                    ViewBag.Customers = Response.Result;
                }
                if (Res3.IsSuccessStatusCode)
                {
                    var Response = Res3.Content.ReadAsAsync<List<Booking>>();
                    Response.Wait();
                    ViewBag.Cancellations = Response.Result;
                }

            }
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> AdmnDashboard(DateTime From, DateTime To)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["FromDate"] = From.Day.ToString();
            data["FromMonth"] = From.Month.ToString();
            data["FromYear"] = From.Year.ToString();
            data["ToDate"] = To.Day.ToString();
            data["ToMonth"] = To.Month.ToString();
            data["ToYear"] = To.Year.ToString();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5093/api/");
                HttpResponseMessage Res1 = await client.GetAsync("AdminAPI/UpcomingThreeDaysBookings/");
                HttpResponseMessage Res2 = await client.GetAsync("AdminAPI/CustRegisteredInSevenDays/");
                HttpResponseMessage Res3 = await client.GetAsync("AdminAPI/CancellationForNextThreedays/");
                HttpResponseMessage Res4 = await client.PostAsJsonAsync("AdminAPI/DateRangeBookings/", data);

                if (Res1.IsSuccessStatusCode)
                {
                    var Response = Res1.Content.ReadAsAsync<List<Booking>>();
                    Response.Wait();
                    ViewBag.Bookings = Response.Result;
                }
                if (Res2.IsSuccessStatusCode)
                {
                    var Response = Res2.Content.ReadAsAsync<List<Customer>>();
                    Response.Wait();
                    ViewBag.Customers = Response.Result;
                }
                if (Res3.IsSuccessStatusCode)
                {
                    var Response = Res3.Content.ReadAsAsync<List<Booking>>();
                    Response.Wait();
                    ViewBag.Cancellations = Response.Result;
                }
                if (Res4.IsSuccessStatusCode)
                {
                    var Response = Res4.Content.ReadAsAsync<List<Booking>>();
                    Response.Wait();
                    ViewBag.DateRangeBookings = Response.Result;
                }

            }            
            TempData["FromDate"] = From;
            TempData["ToDate"] = To;
            return View();
        }

        public async Task<IActionResult> ExportToExcel()
        {
            DateTime From = (DateTime)TempData["FromDate"];
            DateTime To = (DateTime)TempData["ToDate"];
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["FromDate"] = From.Day.ToString();
            data["FromMonth"] = From.Month.ToString();
            data["FromYear"] = From.Year.ToString();
            data["ToDate"] = To.Day.ToString();
            data["ToMonth"] = To.Month.ToString();
            data["ToYear"] = To.Year.ToString();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5093/api/");
                var Res = client.PostAsJsonAsync("AdminAPI/DateRangeBookings/", data);
                Res.Wait();

                var Result = Res.Result;
                if (Result.IsSuccessStatusCode)
                {
                    var Response = Result.Content.ReadAsAsync<List<Booking>>();
                    Response.Wait();
                    var bookings = Response.Result;
                    if (bookings.Count >= 1)
                    {
                        ExcelFileHandling excelFileHandling = new ExcelFileHandling();
                        var stream = excelFileHandling.CreateExcelFile(bookings);
                        string excelName = $"Bookings.xlsx";
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                    }

                }
                return ViewBag.ErrorMessage = "Could not load data";
            }
        }


        public async Task<IActionResult> CustomerBookingDetails()
        {            
            using (var client = new HttpClient())
            {
                List<CustomerBookingModel> customerBookings = new List<CustomerBookingModel>();
                client.BaseAddress = new Uri("http://localhost:5093/api/");
                HttpResponseMessage Res1 = await client.GetAsync("AdminAPI/GetCustomerBookingDetails/");                

                if (Res1.IsSuccessStatusCode)
                {
                    customerBookings = Res1.Content.ReadAsAsync<List<CustomerBookingModel>>().Result;
                }
                return View(customerBookings);
            }           
        }

        [HttpPost]
        public async Task<IActionResult> CustomerBookingDetails(String UserId)
        {
            using (var client = new HttpClient())
            {
                List<CustomerBookingModel> customerBookings = new List<CustomerBookingModel>();
                client.BaseAddress = new Uri("http://localhost:5093/api/");
                var Res = client.PostAsJsonAsync("AdminAPI/GetCustomerBookingByUserId/", UserId);
                Res.Wait();

                var Result = Res.Result;
                if (Result.IsSuccessStatusCode)
                {
                    customerBookings = Result.Content.ReadAsAsync<List<CustomerBookingModel>>().Result;
                }
                return View(customerBookings);
            }           
        }

        public async Task<ActionResult> CheckedIn(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5093/api/");
                
                var Res = client.PostAsJsonAsync("AdminAPI/CheckIn/", id);
                Res.Wait();

                var Result = Res.Result;
                if (Result.IsSuccessStatusCode)
                {
                    return RedirectToAction("CustomerBookingDetails", "AdminDashboard");
                }
                ModelState.AddModelError(string.Empty, "Could not update checkout details");
                return RedirectToAction("CustomerBookingDetails", "AdminDashboard");
            }           
        }

        [HttpGet]
        public async Task<ActionResult> CheckedOut(int id)
        {
            CheckIn CheckInInfo = new CheckIn();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5093/api/");
                HttpResponseMessage Res = await client.GetAsync("AdminAPI/GetCheckIn/"+id.ToString());
                if (Res.IsSuccessStatusCode)
                {                  
                    var Response = Res.Content.ReadAsAsync<CheckIn>();
                    Response.Wait();
                    CheckInInfo = Response.Result;
                }
                ModelState.AddModelError(string.Empty, "Could not found CheckIn Id");
                return View(CheckInInfo);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CheckedOut(CheckIn checkIn)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5093/api/");
                CheckInsModel model = new CheckInsModel();
                model.checkin_id = checkIn.CheckinId;
                model.gross_amount = (float)checkIn.GrossAmount;
                model.booking_id = checkIn.BookingId;
                List<Admin> adminDetailsLst = new List<Admin>();
                Booking bookingDetails = new Booking();
                Customer customerDetails = new Customer();

                var Res = client.PostAsJsonAsync("AdminAPI/CheckOut/", model);
                Res.Wait();

                var Result = Res.Result;
                if (Result.IsSuccessStatusCode)
                {
                    HttpResponseMessage Res2 = await client.GetAsync("CustomerAPI/GetBookingsById/" + checkIn.BookingId.ToString());
                    if (Res2.IsSuccessStatusCode)
                    {
                        var Resp = Res2.Content.ReadAsAsync<Booking>();
                        Resp.Wait();

                        bookingDetails = Resp.Result;
                        HttpResponseMessage Res4 = await client.GetAsync("CustomerAPI/GetCustomerById/" + bookingDetails.CustomerId.ToString());
                        if (Res4.IsSuccessStatusCode)
                        {
                            var Respon = Res4.Content.ReadAsAsync<Customer>();
                            Respon.Wait();
                            customerDetails = Respon.Result;
                        }
                    }

                    if (customerDetails != null)
                    {
                        MailRequest mail = new MailRequest();

                        string message = "Dear " + customerDetails.FirstName + " " + customerDetails.LastName + " .<br>" +
                        "Thank you for visting our Restaurant,Your booking id has been closed.<br>" +
                         "Booking Id :" + bookingDetails.BookingId +
                        "<br>Slot Date and time :" + checkIn.CheckOutTime +
                        "<br>Amount:" + checkIn.GrossAmount +
                        "<br> Vist Again" +
                        "<br>Thank You." +
                        "<br>Best regards," +
                        "<br>Sharan";
                        string subject = "Check out has been confirmed";

                        mail.Body = message;
                        mail.Subject = subject;
                        mail.ToEmail = customerDetails.Email;

                        var resp = client.PostAsJsonAsync("LoginAPI/SendEmail/", mail);
                        resp.Wait();

                        var respResult = resp.Result;
                    }
                    return RedirectToAction("CustomerBookingDetails", "AdminDashboard");
                }
                ModelState.AddModelError(string.Empty, "Could not update checkout details");
                
            }
            return RedirectToAction("CustomerBookingDetails", "AdminDashboard");
        }
       

    }
}
