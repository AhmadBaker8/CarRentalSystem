using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Models
{
    public class Review : BaseModel
    {
        [Required]
        public int CarId { get; set; }
        public Car Car { get; set; }

        [Required]
        public int BookingId { get; set; }
        public Booking Booking { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public bool IsVerified { get; set; } = false;
    }
}
