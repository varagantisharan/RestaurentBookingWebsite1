using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer
{
    public class RegistrationModel
    {
        public int customer_id { get; set; }
        public DateTime booking_date { get; set; }
        public int slot_Time { get; set; }
        public string Status { get; set; }
        public DateTime creation_time { get; set; }
    }
}
