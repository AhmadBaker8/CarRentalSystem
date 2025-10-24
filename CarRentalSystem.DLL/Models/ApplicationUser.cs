using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace CarRentalSystem.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? DriverLicenseNumber { get; set; }
        public DateTime? DriverLicenseExpiry {  get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string? CodeResetPassword { get; set; }
        public DateTime? PasswordResetCodeExpiry { get; set; }

        public List<Booking> Bookings { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
    }
}
