using CarRentalSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Requests
{
    public class CarFilterRequest
    {
        public CarType? Type { get; set; }
        public TransmissionType? Transmission { get; set; }
        public FuelType? FuelType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public string? SearchTerm { get; set; } // Make or Model
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
