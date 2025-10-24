using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Responses
{
    public class PaymentResponse
    {
        public string SessionId { get; set; }
        public string ClientSecret { get; set; }
        public string PaymentIntentId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
