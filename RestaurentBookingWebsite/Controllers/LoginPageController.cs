using Entity_Layer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using RestaurentBookingWebsite.Services;
using RestaurentBookingWebsite.Controllers;
using MailKit;
using RestaurentBookingWebsite.DbModels;
using DocumentFormat.OpenXml.Spreadsheet;
//using Session_State.Models;

namespace RestaurentBookingWebsite.Controllers
{
    public class LoginPageController : Controller
    {
        string Baseurl = "http://localhost:5093/api/";
        private ILogin _loginser;
        private readonly IMail mailService;
        public LoginPageController(ILogin loginser, IMail mailService)
        {
            _loginser = loginser;
            this.mailService = mailService;
        }
        public IActionResult SigninUser()
        {
            return View();
        }



        [HttpPost]
        public IActionResult SigninUser(SignInModel model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                var Res = client.PostAsJsonAsync("LoginAPI/SignIn/", model);
                Res.Wait();

                var Result = Res.Result;
                if (Result.IsSuccessStatusCode)
                {
                    var user = Result.Content.ReadAsAsync<AdminsModel>();
                    var role = user.Result.Role;
                    var id = user.Result.admin_id;
                    var name= user.Result.first_name+" "+user.Result.last_name;

                    var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, name) },
                        CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    
                    

                    if (user.Result.Role == "Customer")
                    {
                        TempData["CustomerId"] = user.Result.admin_id;
                        return RedirectToAction("CustDashboard", "CustomerDashboard", new { @id = user.Result.admin_id });

                    }
                    else if (user.Result.Role == "Admin")
                    {
                        HttpContext.Session.SetString("Username", name);
                        TempData["AdminId"] = user.Result.admin_id;
                        return RedirectToAction("AdmnDashboard", "AdminDashboard");//, new { @id = user.Result.admin_id });
                    }
                    else
                    {
                        return RedirectToAction("SigninUser");
                    }
                }
                ModelState.AddModelError(string.Empty, "Could not update checkout details");
                return RedirectToAction("SigninUser");
            }

        }

        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Signup(AdminsModel newuser)
        {
            //var isUseridExists = DbModels.Customer.Any(x => x.userid == newuser.userid);
            //if(isUseridExists)
            //{
            //    ModelState.AddModelError("userid", "user with this userid alredy exists");
            //    return View();
            //}
            try
            {
                if (newuser.password == newuser.confirm_password)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(Baseurl);
                        var Res = client.PostAsJsonAsync("LoginAPI/SignUp/", newuser);
                        Res.Wait();

                        var Result = Res.Result;
                        if (Result.IsSuccessStatusCode)
                        {
                            var user = Result.Content.ReadAsAsync<SignInModel>();
                            if (user != null)
                            {
                                if(user.Result.UserExists==1)
                                {
                                    ViewBag.Message = "UserId Already Exists...";
                                    return View(newuser);
                                }
                                else
                                {
                                    MailRequest mail = new MailRequest();
                                    // send configuration mail
                                    mail.Body = "Dear " + newuser.first_name + " " + newuser.last_name + ".<br>" +
                                    "Here is your userId please login with this userId " + user.Result.UserId + "." +
                                    "<br>Thank You." +
                                    "<br>Best Regards";
                                    mail.Subject = "Account has been created";
                                    mail.ToEmail = newuser.email;
                                    var resp = client.PostAsJsonAsync("LoginAPI/SendEmail/", mail);
                                    Res.Wait();

                                    var respResult = resp.Result;
                                    if (respResult.IsSuccessStatusCode)
                                    {
                                        ViewBag.Message = "SignUp Successfully Submited";
                                        return RedirectToAction("Signup");
                                    }
                                    else
                                    {
                                        throw new Exception("Email is not sent");
                                    }
                                }                                                                                            
                            }
                            else
                            {
                                throw new Exception("Signup is not successful");
                            }
                        }
                        ModelState.AddModelError(string.Empty, "Signup is not successful");
                        //return RedirectToAction("Signup");
                        
                    }
                    return ViewBag.Message;
                }
                else
                {
                    throw new Exception("Password and Confirm Password must be same");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Signup failed. Please try again later.");
                throw new Exception("Signup is not successful");
            }
        }

        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SigninUser");
        }

    }
}
