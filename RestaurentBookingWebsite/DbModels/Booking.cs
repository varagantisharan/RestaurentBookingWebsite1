using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurentBookingWebsite.DbModels;

public partial class Booking
{
    public int BookingId { get; set; }

    public int CustomerId { get; set; }

    public DateTime BookingDate { get; set; }

    public int SlotTime { get; set; }

    public string? Status { get; set; }

    public DateTime? CreationTime { get; set; }

    [JsonIgnore]
    public virtual ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();

    [JsonIgnore]
    public virtual Customer Customer { get; set; } = null!;
}
