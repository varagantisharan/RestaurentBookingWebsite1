using Entity_Layer;
using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestaurentBookingWebsite.DbModels;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RestaurentBookingWebsite.Services
{                                                                              
    public class BookingServices : IBooking
    {
        private RestaurantContext db;
        private readonly IMail mailService;
        public BookingServices(RestaurantContext db, IMail mailService)
        {
            this.db = db;
            this.mailService = mailService; 
        }
        public Booking Register(BookingsModel model)
        { 
            Booking MyBooking = new Booking();           
            MyBooking.CustomerId= model.customer_id;

            string bookingDate = model.date.ToString() +"/"+ model.month.ToString() + "/" + DateTime.Now.Year.ToString() +" "+ model.slot_Time;
            //string formattedDate = model.booking_date.ToShortDateString() + " " + model.slot_Time;
            //model.booking_date = Convert.ToDateTime(formattedDate);
            MyBooking.BookingDate = Convert.ToDateTime(bookingDate);

            if (model.slot_Time == "09:00:00")
            {
                MyBooking.SlotTime = 1;
            }
            else if (model.slot_Time == "13:00:00")
            {
                MyBooking.SlotTime = 2;
            }
            else if (model.slot_Time == "17:00:00")
            {
                MyBooking.SlotTime = 3;
            }
            else
            {
                MyBooking.SlotTime = 4;
            }

            try
            {
                db.Bookings.Add(MyBooking);
                db.SaveChanges();              

                return db.Bookings.Where(b=> b.CustomerId==model.customer_id && b.BookingDate==MyBooking.BookingDate && b.SlotTime==MyBooking.SlotTime).OrderByDescending(b=>b.CreationTime).FirstOrDefault();
                //return 1;
               
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
        public Customer GetCustomerDetails(int id)
        {
            var IsValidCustomer = db.Customers.Find(id);
            if(IsValidCustomer!=null)
            {
                return IsValidCustomer;
            }
            else
            {
                throw new Exception("Customer not Found");
            }
        }
       
        public Booking GetBookingDetails(int id)
        {
            var bookingDetails = db.Bookings.Find(id);
            if(bookingDetails!=null)
            {
                return bookingDetails;
            }
            else
            { 
                throw new Exception("No booking details found for this customer");
            }
        }

        public List<Booking> GetCustomerBookingDetails(int custId)
        {
            var bookingDetails = db.Bookings.Where(b => b.CustomerId == custId).OrderByDescending(p => p.BookingId).ToList();
            if (bookingDetails != null)
            {
                return bookingDetails;
            }
            else
            {
                throw new Exception("No booking details found for this customer");
            }
        }

        public int CancelBooking(int bookingId)
        {
            if(bookingId!=0)
            {
                var bookingDetails = db.Bookings.Find(bookingId);
                if(bookingDetails!=null)
                {
                    DateTime currentTime = DateTime.Now;
                    var hrs = bookingDetails.BookingDate.Subtract(currentTime);
                    TimeSpan time = new TimeSpan(24,0,0);
                    if (hrs > time)
                    {
                        bookingDetails.Status = "Cancelled";
                        db.Entry(bookingDetails).State = EntityState.Modified;
                        db.SaveChanges();                      
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    throw new Exception("No booking details found.");
                }
                
            }
            else
            {
                throw new Exception("No booking details found with the given Booking Id.");
            }
        }
    }



}

