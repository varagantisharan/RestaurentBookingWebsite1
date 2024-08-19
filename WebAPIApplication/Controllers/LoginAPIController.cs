using Entity_Layer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurentBookingWebsite.Services;

namespace RestaurentBookingWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginAPIController : ControllerBase
    {
        private ILogin _loginser;
        private readonly IMail _mailService;
        public LoginAPIController(ILogin loginser, IMail mailService)
        {
            _loginser = loginser;
            _mailService = mailService;
        }

        [HttpPost]
        [Route("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInModel model)
        {
            var user = _loginser.SignIn(model);
            if (user == null)
            {
                return BadRequest("Invalid Credentials");
            }
            return Ok(user);
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] AdminsModel newuser)
        {
            SignInModel user = new SignInModel();
            if (newuser.Role == "Admin")
            {
                user = _loginser.AdminSignUp(newuser);
            }
            else if (newuser.Role == "Customer")
            {
                var customer = new CustomersModel();
                customer.userid = newuser.userid;
                customer.first_name = newuser.first_name;
                customer.last_name = newuser.last_name;
                customer.address = newuser.address;
                customer.password = newuser.password;
                customer.phone_number = newuser.phone_number;
                customer.email = newuser.email;
                customer.confirm_password = newuser.confirm_password;
                customer.role = newuser.Role;
                user = _loginser.CustomerSignUp(customer);
            }
            else
            {
                return BadRequest("Role is Mandatory");
            }
            if (user != null)
            {
                return Ok(user);
            }
            return BadRequest("Signup is not successful");
        }

        [HttpPost]
        [Route("SendEmail")]
        public async Task<IActionResult> SendEmail(MailRequest mail)
        {
            try
            {
                var res = _mailService.SendMail(mail);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message + " " + ex.Message);
            }
        }
    }
}
