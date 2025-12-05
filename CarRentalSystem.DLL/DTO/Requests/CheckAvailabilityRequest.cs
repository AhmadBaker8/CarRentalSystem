using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Requests
{
    public class CheckAvailabilityRequest
    {
        public int CarId { get; set; }
        public DateTime pickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}
