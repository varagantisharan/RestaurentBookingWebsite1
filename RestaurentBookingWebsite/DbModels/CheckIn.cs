using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurentBookingWebsite.DbModels;

public partial class CheckIn
{
    public int CheckinId { get; set; }

    public int BookingId { get; set; }

    public DateTime? CheckinTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public int? GrossAmount { get; set; }

    [JsonIgnore]
    public virtual Booking Booking { get; set; } = null!;
}
