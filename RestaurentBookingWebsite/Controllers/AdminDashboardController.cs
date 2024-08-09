using Microsoft.AspNetCore.Mvc;
using RestaurentBookingWebsite.Services;
using System.IO;
using System.Data;
using System.Web;
using System.IO;
using Microsoft.AspNetCore.Http;
using RestaurentBookingWebsite.DbModels;
using Entity_Layer;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Diagnostics.CodeAnalysis;
using Humanizer;
using DocumentFormat.OpenXml.Bibliography;
using RestaurentBookingWebsite.Models;
using RestaurentBookingWebsite.Controllers;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Net.Http.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;


namespace RestaurentBookingWebsite.Controllers
{
    [Authorize]
    public class AdminDashboardController : Controller
    {
        string Baseurl = "http://localhost:5093/api/";
        private ILogin _loginService;
        private IAdmin _adminService;

        int admin_id;
        public AdminDashboardController(ILogin loginService, IAdmin adminService)
        {
            _loginService = loginService;
            _adminService = adminService;
        }
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
            //ViewBag.Bookings = _adminService.UpcomingThreeDaysBookings();
            //ViewBag.Customers = _adminService.CustRegisteredInSevenDays();
            //ViewBag.Cancellations = _adminService.CancellationForNextThreedays();
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> AdmnDashboard(DateTime From, DateTime To)
        {
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
            var bookings = _adminService.BookingsAsPerDateRange(From,To);
            TempData["FromDate"] = From;
            TempData["ToDate"] = To;
            ViewBag.DateRangeBookings = bookings;
            return View(bookings);
        }

        public IActionResult ExportToExcel()
        {
            DateTime From = (DateTime)TempData["FromDate"];
            DateTime To = (DateTime)TempData["ToDate"];
            var bookings = _adminService.BookingsAsPerDateRange(From, To);
            ExcelFileHandling excelFileHandling = new ExcelFileHandling();
            var stream = excelFileHandling.CreateExcelFile(bookings);
            string excelName = $"Bookings.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }


        public IActionResult CustomerBookingDetails()
        {
            List <Booking> bookings = _adminService.UpcomingThreeDaysBookings();
            List<Customer> customers = _adminService.GetBookedCustomerDetails(bookings);
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

            return View(customerBookings);
        }

        [HttpPost]
        public IActionResult CustomerBookingDetails(String UserId)
        {
            List<Booking> bookings = _adminService.UpcomingThreeDaysBookings();
            List<Customer> customers = _adminService.GetBookedCustomerDetails(bookings).Where(c => c.UserId==UserId).ToList();
            List<CheckIn> checkins = _adminService.GetAllCheckIns();

            var customerBookings = from c in customers
                                   join b in bookings on c.CustomerId equals b.CustomerId into table1
                                   from b in table1
                                   join ch in checkins on b.BookingId equals ch.BookingId into table2
                                   from ch in table2.DefaultIfEmpty(). ToList()    
                                   select new CustomerBookingModel
                                   {
                                       Customer = c,
                                       Booking = b,
                                       CheckIn = ch,
                                   };
            return View(customerBookings);
        }

        public async Task<ActionResult> CheckedIn(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                
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
                //http://localhost:5093/api/AdminAPI/CheckOut
                client.BaseAddress = new Uri(Baseurl);
                CheckInsModel model = new CheckInsModel();
                model.checkin_id = checkIn.CheckinId;
                model.gross_amount = (float)checkIn.GrossAmount;
                model.booking_id = checkIn.BookingId;

                var Res = client.PostAsJsonAsync("AdminAPI/CheckOut/", model);
                Res.Wait();

                var Result = Res.Result;
                if (Result.IsSuccessStatusCode)
                {
                    return RedirectToAction("CustomerBookingDetails", "AdminDashboard");
                }
                ModelState.AddModelError(string.Empty, "Could not update checkout details");
                
            }
            return RedirectToAction("CustomerBookingDetails", "AdminDashboard");
        }
       

    }
}
