using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurentBookingWebsite.DbModels;

public partial class Customer
{
    public int CustomerId { get; set; }
    public string UserId { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Address { get; set; }

    public string Password { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

   
    public DateTime? DateOfRegistration { get; set; }

    [JsonIgnore]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
