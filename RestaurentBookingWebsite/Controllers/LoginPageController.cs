using Entity_Layer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace RestaurentBookingWebsite.Controllers
{
    public class LoginPageController : Controller
    {
        string Baseurl = "http://localhost:5048/api/";
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
                        HttpContext.Session.SetString("CustomerName", name);
                        HttpContext.Session.SetInt32("CustomerId", id);
                        return RedirectToAction("CustDashboard", "CustomerDashboard", new { @id = user.Result.admin_id });

                    }
                    else if (user.Result.Role == "Admin")
                    {
                        HttpContext.Session.SetString("Username", name);
                        return RedirectToAction("AdmnDashboard", "AdminDashboard");
                    }
                    else
                    {
                        return RedirectToAction("SigninUser");
                    }
                }
               
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
                                        ViewBag.Message1 = "SignUp Successfully Submited";
                                        return View(newuser);
                                        //return RedirectToAction("SigninUser");
                                    }                                    
                                }                                                                                            
                            }                          
                        }                       
                    }
                    return ViewBag.Message;
                }
                else
                {
                    ViewBag.ErrorMessage = "Password and Confirm Password must be same.";
                    return View(newuser);
                }
            }
            catch (Exception e)
            {               
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
