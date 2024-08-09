using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer
{
    public class SignInModel
    {
        public string UserId { get; set; } 
        public string Password { get; set; } 
        public int Id { get; set; }
        public int UserExists { get; set; }
    }
}
