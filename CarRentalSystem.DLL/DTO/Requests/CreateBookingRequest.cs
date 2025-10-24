using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Requests
{
    public class CreateBookingRequest
    {
        [Required(ErrorMessage = "Car ID is required")]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Pickup date is required")]
        [DataType(DataType.DateTime)]
        public DateTime PickupDate { get; set; }

        [Required(ErrorMessage = "Return date is required")]
        [DataType(DataType.DateTime)]
        public DateTime ReturnDate { get; set; }

        [Required(ErrorMessage = "Pickup location is required")]
        [MaxLength(200)]
        public string PickupLocation { get; set; }

        [Required(ErrorMessage = "Return location is required")]
        [MaxLength(200)]
        public string ReturnLocation { get; set; }

        [Required(ErrorMessage = "Driver name is required")]
        [MaxLength(100)]
        public string DriverName { get; set; }

        [Required(ErrorMessage = "Driver license number is required")]
        [MaxLength(50)]
        public string DriverLicenseNumber { get; set; }

        [Required(ErrorMessage = "Driver license expiry is required")]
        public DateTime DriverLicenseExpiry { get; set; }

        [Required(ErrorMessage = "Contact phone is required")]
        [Phone]
        public string ContactPhone { get; set; }

        public bool HasInsurance { get; set; } = false;
        public bool NeedsGPS { get; set; } = false;

        [MaxLength(500)]
        public string? SpecialRequests { get; set; }
    }
}
