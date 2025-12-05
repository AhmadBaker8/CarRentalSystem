using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Requests
{
    public class CalculatePriceRequest
    {
        public int CarId { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool HasInsurance { get; set; }
        public bool NeedsGPS { get; set; }
        public bool NeedsChildSeat { get; set; }
    }
}
