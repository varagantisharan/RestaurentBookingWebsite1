﻿using System;
using System.Collections.Generic;

namespace RestaurentBookingWebsite.DbModels;

public partial class Admin
{
    public int AdminId { get; set; }

    public string UserId { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

   
    public DateTime? DateOfRegistration { get; set; }
}
