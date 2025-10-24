using CarRentalSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Responses
{
    public class CarResponse
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public string PlateNumber { get; set; }
        public CarType Type { get; set; }
        public TransmissionType Transmission { get; set; }
        public int SeatingCapacity { get; set; }
        public FuelType FuelType { get; set; }
        public int Mileage { get; set; }
        public decimal DailyRate { get; set; }
        public decimal WeeklyRate { get; set; }
        public decimal MonthlyRate { get; set; }
        public CarStatus Status { get; set; }
        public string? Description { get; set; }

        // Features
        public bool HasAirConditioning { get; set; }
        public bool HasGPS { get; set; }
        public bool HasBluetooth { get; set; }
        public bool HasBackupCamera { get; set; }
        public bool HasSunroof { get; set; }

        // Images
        public List<CarImageResponse> Images { get; set; } = new();

        // Rating
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class CarImageResponse
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
    }

    public class CarDetailResponse : CarResponse
    {
        public List<ReviewResponse> Reviews { get; set; } = new();
    }

   
}
