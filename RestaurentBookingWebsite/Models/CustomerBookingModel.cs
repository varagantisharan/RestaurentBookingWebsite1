using RestaurentBookingWebsite.DbModels;

namespace RestaurentBookingWebsite.Models
{
    public class CustomerBookingModel
    {
        public Customer Customer { get; set; }
        public Booking Booking { get; set; }
        public CheckIn CheckIn { get; set; }
    }
}
