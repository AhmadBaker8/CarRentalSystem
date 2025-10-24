using CarRentalSystem.DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Requests
{
    public class AddCarRequest
    {
        [Required(ErrorMessage = "Make is required")]
        [MaxLength(100)]
        public string Make { get; set; }

        [Required(ErrorMessage = "Model is required")]
        [MaxLength(100)]
        public string Model { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(1980, 2050)]
        public int Year { get; set; }

        [Required(ErrorMessage = "Color is required")]
        [MaxLength(50)]
        public string Color { get; set; }

        [Required(ErrorMessage = "Plate number is required")]
        [MaxLength(20)]
        public string PlateNumber { get; set; }

        [Required]
        public CarType Type { get; set; }

        [Required]
        public TransmissionType Transmission { get; set; }

        [Required(ErrorMessage = "Seating capacity is required")]
        [Range(1, 15)]
        public int SeatingCapacity { get; set; }

        [Required]
        public FuelType FuelType { get; set; }

        [Range(0, int.MaxValue)]
        public int Mileage { get; set; }

        [Required(ErrorMessage = "Daily rate is required")]
        [Range(0.01, double.MaxValue)]
        public decimal DailyRate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal WeeklyRate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MonthlyRate { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public IFormFile MainImage { get; set; }
        public List<IFormFile> SubImages { get; set; } = new List<IFormFile>();

        // Features
        public bool HasAirConditioning { get; set; }
        public bool HasGPS { get; set; }
        public bool HasBluetooth { get; set; }
        public bool HasBackupCamera { get; set; }
        public bool HasSunroof { get; set; }
    }
}
