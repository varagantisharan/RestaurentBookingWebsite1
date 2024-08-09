using DocumentFormat.OpenXml.Bibliography;
using Entity_Layer;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurentBookingWebsite.DbModels;

namespace RestaurentBookingWebsite.Services
{
    public class AdminServices : IAdmin
    {
        private RestaurantContext db;
        private readonly IMail mailService;
        public AdminServices(RestaurantContext db, IMail mailService) 
        {
            this.db = db;
            this.mailService = mailService;
        }
        public List<Customer> CustRegisteredInSevenDays()
        {
            DateTime minsSevenDays = DateTime.Now.AddDays(-7);
            DateTime currentDay = DateTime.Now;

            var customers = db.Customers.Where(c => c.DateOfRegistration<=currentDay && c.DateOfRegistration>=minsSevenDays).ToList();
            if(customers!=null)
            {
                return customers;
            }
            else
            {
                throw new Exception("No details found");
            }
        }

        public List<Booking> UpcomingThreeDaysBookings()
        {
            DateTime nextthreedays = DateTime.Now.AddDays(3).Date;
            DateTime currentDay = DateTime.Now.Date;

            var bookings = db.Bookings.Where(b => b.BookingDate.Date>= currentDay && b.BookingDate.Date <= nextthreedays && b.Status=="Booked").ToList();
            if (bookings != null)
            {
                return bookings;
            }
            else
            {
                throw new Exception("No details found");
            }
        }

        public List<Booking> BookingsAsPerDateRange(DateTime from, DateTime to)
        {
            var bookings = db.Bookings.Where(b => b.BookingDate >= from && b.BookingDate <= to).ToList();
            if (bookings != null)
            {
                return bookings;
            }
            else
            {
                throw new Exception("No details found");
            }
        }

        public List<Booking> CancellationForNextThreedays()
        {
            DateTime nextthreedays = DateTime.Now.AddDays(3);
            DateTime currentDay = DateTime.Now;

            var Cancellations = db.Bookings.Where(b => b.BookingDate >= currentDay && b.BookingDate <= nextthreedays && b.Status == "Cancelled").ToList();
            if (Cancellations != null)
            {
                return Cancellations;
            }
            else
            {
                throw new Exception("No details found");
            }
        }

        public List<Customer> GetBookedCustomerDetails(List<Booking> bookingmodel)
        {
            return db.Customers.ToList();
        }

        public List<CheckIn> GetAllCheckIns()
        {
            return db.CheckIns.ToList();    
        }

        public int UpdateCheckInDetails(int BookingId)
        {
            try
            {
                if (BookingId > 0)
                {
                    var BookingExists = db.Bookings.Find(BookingId);
                    if (BookingExists != null)
                    {
                        if(DateTime.Now>=BookingExists.BookingDate && DateTime.Now<=BookingExists.BookingDate.AddDays(1))
                        {
                            var checkInExists = db.CheckIns.Where(c => c.BookingId == BookingId).ToList();
                            if (checkInExists.Count == 0)
                            {
                                CheckIn checkInModel = new CheckIn();
                                checkInModel.BookingId = BookingId;
                                db.CheckIns.Add(checkInModel);
                                db.SaveChanges();
                                return 1;
                            }
                            else
                            {
                                throw new Exception("Checkin already exists");
                            }
                        }
                        else
                        {
                            throw new Exception("Checkin is only permitted at the booked slot time");
                        }                        
                    }
                    else
                    {
                        throw new Exception("Booking Id does not exist in the checkin table");
                    }
                }
                else
                {
                    throw new Exception("Invalid Booking Id");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }

        public int UpdateCheckOutDetails(int id, int amt,int bookingId)
        {
            if (id > 0)
            {
                var bookingdetails = db.Bookings.Find(bookingId);
                var checkOutExists = db.CheckIns.Find(id);
                if(checkOutExists != null)
                {
                    if(DateTime.Now >= checkOutExists.CheckinTime && DateTime.Now.Date==checkOutExists.CheckinTime.Value.Date)
                    {
                        checkOutExists.CheckOutTime = DateTime.Now;
                        //checkOutExists.GrossAmount = model.GrossAmount;
                        checkOutExists.GrossAmount = amt;
                        db.Entry(checkOutExists).State = EntityState.Modified;
                        db.SaveChanges();


                        var customerdetails = db.Customers.Find(db.Bookings.Find(checkOutExists.BookingId).CustomerId);
                       
                        if(customerdetails != null)
                        {
                            MailRequest mail = new MailRequest();

                            string message = "Dear " + customerdetails.FirstName + " " + customerdetails.LastName + " .<br>" +
                            "Thank you for visting our Restaurant,Your booking id has been closed.<br>" +
                             "Booking Id :" + bookingdetails.BookingId +
                            "<br>Slot Date and time :" + checkOutExists.CheckOutTime +
                            "<br>Amount:" + checkOutExists.GrossAmount +
                            "<br> Vist Again" +
                            "<br>Thank You." +
                            "<br>Best regards," +
                            "<br>Sharan";
                            string subject = "Check out has been confirmed";

                            mail.Body = message;
                            mail.Subject = subject;
                            mail.ToEmail = customerdetails.Email;

                            mailService.SendEmail(mail);
                        }



                        return 1;
                    }
                    else
                    {
                        throw new Exception("Invalid Checkout time");
                    }
                } 
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public CheckIn GetCheckIn(int id)
        {
            var CheckInDetails = db.CheckIns.Find(id);
            if(CheckInDetails != null)
            {
                return CheckInDetails;
            }
            else
            {
                throw new Exception("CheckIn Id not found");
            }
        }
    }
}
