using CarRentalSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Requests
{
    public class UpdateBookingStatusRequest
    {
        [Required]
        public BookingStatus Status { get; set; }
    }
}
