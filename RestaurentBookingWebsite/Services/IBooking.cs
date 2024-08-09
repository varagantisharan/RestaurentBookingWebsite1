using  Entity_Layer;
using RestaurentBookingWebsite.DbModels;

namespace RestaurentBookingWebsite.Services
{
    public interface IBooking
    {
        public int Register(BookingsModel model);
        public Customer GetCustomerDetails(int id);
        public List<Admin> GetAllAdminDetails();
        public Booking GetBookingDetails(int custId);
        public List<Booking> GetCustomerBookingDetails(int custId);
        public int CancelBooking(int bookingId);
    }
}
