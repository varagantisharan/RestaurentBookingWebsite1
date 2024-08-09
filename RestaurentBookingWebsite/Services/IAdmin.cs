using Entity_Layer;
using Microsoft.AspNetCore.Mvc;
using RestaurentBookingWebsite.DbModels;

namespace RestaurentBookingWebsite.Services
{
    public interface IAdmin
    {
        public List<Customer> CustRegisteredInSevenDays();
        public List<Booking> UpcomingThreeDaysBookings();
        public List<Booking> CancellationForNextThreedays();
        public List<Booking> BookingsAsPerDateRange(DateTime from, DateTime to);
        public List<Customer> GetBookedCustomerDetails(List<Booking> bookingmodel);
        public List<CheckIn> GetAllCheckIns();

        public CheckIn GetCheckIn(int id);
        public int UpdateCheckInDetails(int BookingId);
        public int UpdateCheckOutDetails(int id, int amt,int bookingId);

    }
}
