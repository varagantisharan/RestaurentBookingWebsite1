using Entity_Layer;

namespace RestaurentBookingWebsite.Services
{
    public interface ILogin
    {
        public SignInModel AdminSignUp(AdminsModel register);
        public SignInModel CustomerSignUp(CustomersModel register);
        public AdminsModel SignIn(SignInModel login); 


        public string Encryptdata(string password);
        public string Decryptdata(string encryptpwd);
       
        
    }
}
