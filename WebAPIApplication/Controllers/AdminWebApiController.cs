using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurentBookingWebsite;

namespace WebAPIApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminWebApiController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private IAdmin _adminService;
        public AdminWebApiController(IAdmin adminService)
        {
            _adminService = adminService;

        }


    }
}
