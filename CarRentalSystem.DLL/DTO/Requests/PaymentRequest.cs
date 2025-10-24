using CarRentalSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Requests
{
    public class PaymentRequest
    {
        [Required]
        public int BookingId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string BookingDescription { get; set; }
        [Required]
        public PaymentMethodEnum PaymentMethod { get; set; }
    }
}
