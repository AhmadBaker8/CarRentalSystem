using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Responses
{
    public class ProfileResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }
}
