using CarRentalSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Responses
{
    public class BookingResponse
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string CarMakeModel { get; set; }
        public string CarImage { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }

        public string PickupLocation { get; set; }
        public string ReturnLocation { get; set; }

        public int RentalDays { get; set; }

        public decimal BasePrice { get; set; }
        public decimal InsurancePrice { get; set; }
        public decimal AdditionalCharges { get; set; }
        public decimal LateFee { get; set; }
        public decimal DamageFee { get; set; }
        public decimal TotalPrice { get; set; }

        public BookingStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? PaymentId { get; set; }

        public bool HasInsurance { get; set; }
        public bool NeedsGPS { get; set; }
        public string? SpecialRequests { get; set; }

        // Driver Info
        public string DriverName { get; set; }
        public string DriverLicenseNumber { get; set; }
        public DateTime DriverLicenseExpiry { get; set; }
        public string ContactPhone { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class BookingSummaryResponse
    {
        public int Id { get; set; }
        public string CarMakeModel { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}
