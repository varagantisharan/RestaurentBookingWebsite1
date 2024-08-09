using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurentBookingWebsite.Services;

namespace WebAPIApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminServicesAPIController : ControllerBase
    {
        private ILogin _loginService;
        private IAdmin _adminService;
    }
}
