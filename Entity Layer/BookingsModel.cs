using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entity_Layer
{
    public class BookingsModel
    {
        public int customer_id { get; set; }

        //[DataType(DataType.Date)]

        //[DataType(DataType.DateTime)]
        //[Display(Name = "Date Edit")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd-MM-yyyy}")]
        [JsonIgnore]
        public DateTime booking_date { get; set; }
        public string slot_Time { get; set; }
        public string Status { get; set; }

        [JsonIgnore]
        public DateTime creation_time { get; set; }

        public int date { get; set; }
        public int month { get; set; }
    }
                        
       
}
