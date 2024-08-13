using DocumentFormat.OpenXml.Spreadsheet;
using Entity_Layer;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Protocol;
using NuGet.Protocol.Plugins;
using RestaurentBookingWebsite.DbModels;
using System.Text;

namespace RestaurentBookingWebsite.Services
{
    public class LoginService : ILogin
    {
        private RestaurantContext db;

        public LoginService(RestaurantContext db)
        {
            this.db = db;
        }

     

        public SignInModel AdminSignUp(AdminsModel register)
        {
            string Encypted_Password = Encryptdata(register.password);

            Admin newuser = new Admin();
            //newuser.AdminId = register.admin_id;
            newuser.UserId = register.userid;
            newuser.FirstName = register.first_name;
            newuser.LastName = register.last_name;
            newuser.Password = Encypted_Password;
            newuser.Email = register.email;
            newuser.Address = register.address;
            newuser.PhoneNumber = register.phone_number;


            SignInModel user = new SignInModel();
            try
            {
                var AdminExists = db.Customers.Where(c => c.UserId == newuser.UserId).ToList();
                if (AdminExists.Count==0)
                {
                    db.Admins.Add(newuser);
                    db.SaveChanges();

                    var CreatedUser = db.Admins.OrderBy(p => p.AdminId).Last();

                    user.UserId = register.userid;
                    user.Password = register.password;
                    user.Id = CreatedUser.AdminId;
                    return user;
                }
                else
                {
                    user.UserExists = 1;
                    return user;
                }
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    throw new Exception(e.InnerException.Message);
                }
                throw new Exception(e.Message);
            }


        }
        public SignInModel CustomerSignUp(CustomersModel register)
        {
            string Encypted_Password = Encryptdata(register.password);

            Customer newuser = new Customer();
            newuser.UserId = register.userid;
            newuser.FirstName = register.first_name;
            newuser.LastName = register.last_name;
            newuser.Address = register.address;
            newuser.Password = Encypted_Password;
            newuser.PhoneNumber = register.phone_number;
            newuser.Email = register.email;

            SignInModel user = new SignInModel();
            try
            {
                var IsExistingCustomer = db.Customers.Where(c => c.UserId == newuser.UserId).ToList();

                if(IsExistingCustomer.Count == 0)
                {
                    db.Customers.Add(newuser);
                    db.SaveChanges();

                    var CreatedUser = db.Customers.OrderBy(p => p.CustomerId).Last();

                    user.UserId = CreatedUser.UserId;
                    user.Password = register.password;
                    user.Id = CreatedUser.CustomerId;

                    return user;
                }
                else
                {
                    user.UserExists = 1;
                    return user;
                }
               
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    throw new Exception(e.InnerException.Message);
                }
                throw new Exception(e.Message);
            }

        }
        public AdminsModel SignIn(SignInModel login)
        {
            AdminsModel model = new AdminsModel();
            try
            {
                var IsCustomer = db.Customers.Where(c => c.UserId == login.UserId).ToList();
                //Find(login.UserId);
                var IsAdmin = db.Admins.Where(c => c.UserId == login.UserId).ToList();
                //.Find(login.UserId);

                if (IsCustomer.Count>=1)
                {                   
                    string Decrypted_Password = Decryptdata(IsCustomer[0].Password);
                    if ((IsCustomer[0].UserId == login.UserId) & (login.Password == Decrypted_Password))
                    {
                        //model.userid = IsCustomer.UserId;
                        model.admin_id = IsCustomer[0].CustomerId;
                        model.Role = "Customer";
                        model.first_name = IsCustomer[0].FirstName;
                        model.last_name = IsCustomer[0].LastName;
                        return model;
                    }
                    else
                    {
                        throw new Exception("Invalid username or password");
                    }
                }
                else if (IsAdmin.Count >= 1)
                {
                    string Decrypted_Password = Decryptdata(IsAdmin[0].Password);
                    if ((IsAdmin[0].UserId == login.UserId) & (login.Password == Decrypted_Password))
                    {
                        //model.u = IsAdmin.UserId;
                        model.admin_id = IsAdmin[0].AdminId;
                        model.Role = "Admin";
                        model.first_name= IsAdmin[0].FirstName;
                        model.last_name= IsAdmin[0].LastName;
                        return model;
                    }
                    else
                    {
                        throw new Exception("Invalid username or password");
                    }

                }
                else
                {
                    throw new Exception("Enter valid details");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }


        public string Encryptdata(string password)
        {
            string strmsg = string.Empty;
            byte[] encode = new byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }

        public string Decryptdata(string encryptpwd)
        {
            string decryptpwd = string.Empty;
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptpwd);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            return decryptpwd;
        }

       
        
    }
}
