using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Models
{
    public class Car : BaseModel
    {
        [Required]
        [MaxLength(100)]
        public string Make { get; set; }

        [Required]
        [MaxLength(100)]
        public string Model { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [MaxLength(50)]
        public string Color { get; set; }

        [Required]
        [MaxLength(20)]
        public string PlateNumber { get; set; }

        [Required]
        public CarType Type { get; set; }

        [Required]
        public TransmissionType Transmission { get; set; }

        [Required]
        public int SeatingCapacity { get; set; }

        [Required]
        public FuelType FuelType { get; set; }

        public int Mileage { get; set; }

        [Required]
        public decimal DailyRate { get; set; }

        public decimal WeeklyRate { get; set; }
        public decimal MonthlyRate { get; set; }

        public CarStatus Status { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }


        public bool HasAirConditioning { get; set; }
        public bool HasGPS { get; set; }
        public bool HasBluetooth { get; set; }
        public bool HasBackupCamera { get; set; }
        public bool HasSunroof { get; set; }


        public List<CarImage> CarImages { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
        public List<Maintenance> MaintenanceRecords { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
    }

    public enum CarType
    {
        Economy = 1,
        Compact = 2,
        Sedan = 3,
        SUV = 4,
        Luxury = 5,
        Sport = 6,
        Van = 7
    }

    public enum TransmissionType
    {
        Manual = 1,
        Automatic = 2
    }

    public enum FuelType
    {
        Petrol = 1,
        Diesel = 2,
        Electric = 3,
        Hybrid = 4
    }

    public enum CarStatus
    {
        Available = 1,
        Rented = 2,
        Maintenance = 3,
        OutOfService = 4
    }
}
