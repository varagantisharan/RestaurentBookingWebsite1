
using System.ComponentModel.DataAnnotations;


namespace Entity_Layer
{
    public class AdminsModel
    {
        [Required(ErrorMessage = "AdminId is required.")]
        public int admin_id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(20, ErrorMessage = "First name cannot be longer than 20 characters.")]
        public string first_name { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(20, ErrorMessage = "Last name cannot be longer than 20 characters.")]
        public string last_name { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string password{ get; set; }

        [Required(ErrorMessage = "Cinfirm Password is required.")]
        public string confirm_password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string email { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; }

        [StringLength(50, ErrorMessage = "Address cannot be longer than 50 characters.")]
        public string address { get; set; }


        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string phone_number { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        [StringLength(20, ErrorMessage = "User ID cannot be longer than 50 characters.")]
        public string userid { get; set; }

    }
}
