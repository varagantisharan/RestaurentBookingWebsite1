using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer
{
   public class CheckInsModel
    {
        public int checkin_id { get; set; }
        public int booking_id { get; set; }
        public DateTime checkin_time { get; set; }
        public DateTime check_out_time { get; set; }
        public float gross_amount { get; set; }

    }
}
