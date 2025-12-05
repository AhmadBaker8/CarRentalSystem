using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Models
{
    public class Booking :BaseModel
    {
        [Required]
        public int CarId { get; set; }
        public Car Car { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public DateTime PickupDate { get; set; }

        [Required]
        public DateTime ReturnDate { get; set; }

        public DateTime? ActualReturnDate { get; set; }

        [Required]
        [MaxLength(200)]
        public string PickupLocation { get; set; }

        [Required]
        [MaxLength(200)]
        public string ReturnLocation { get; set; }

        public int RentalDays { get; set; }

        public decimal BasePrice { get; set; }
        public decimal InsurancePrice { get; set; }
        public decimal AdditionalCharges { get; set; }
        public decimal LateFee { get; set; }
        public decimal DamageFee { get; set; }
        public decimal TotalPrice { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public string? PaymentId { get; set; }

        public bool HasInsurance { get; set; }
        public bool NeedsGPS { get; set; }
        public bool NeedsChildSeat { get; set; }

        [MaxLength(500)]
        public string? SpecialRequests { get; set; }

        // Driver Info
        [Required]
        [MaxLength(100)]
        public string DriverName { get; set; }

        [Required]
        [MaxLength(50)]
        public string DriverLicenseNumber { get; set; }

        [Required]
        public DateTime DriverLicenseExpiry { get; set; }

        [Required]
        [Phone]
        public string ContactPhone { get; set; }

        // Inspection
        public int? PickupMileage { get; set; }
        public int? ReturnMileage { get; set; }
        public string? PickupConditionNotes { get; set; }
        public string? ReturnConditionNotes { get; set; }

        // Relations
        public List<DamageReport> DamageReports { get; set; } = new();
    }

    public enum BookingStatus
    {
        [EnumMember(Value = "Pending")]
        Pending = 1,
        [EnumMember(Value = "Confirmed")]
        Confirmed = 2,
        [EnumMember(Value = "Active")]
        Active = 3,
        [EnumMember(Value = "Completed")]
        Completed = 4,
        [EnumMember(Value = "Cancelled")]
        Cancelled = 5,
        [EnumMember(Value = "NoShow")]
        NoShow = 6
    }

    public enum PaymentStatus
    {
        [EnumMember(Value = "Pending")]
        Pending = 1,
        [EnumMember(Value = "Paid")]
        Paid = 2,
        [EnumMember(Value = "PartiallyPaid")]
        PartiallyPaid = 3,
        [EnumMember(Value = "Refunded")]
        Refunded = 4,
        [EnumMember(Value = "Failed")]
        Failed = 5
    }
    public enum PaymentMethodEnum
    {
        [EnumMember(Value = "Cash")]
        Cash = 1,
        [EnumMember(Value = "Visa")]
        Visa = 2
    }
}
