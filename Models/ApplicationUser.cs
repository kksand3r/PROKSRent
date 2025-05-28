﻿using Microsoft.AspNetCore.Identity;

namespace PROKSRent.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
