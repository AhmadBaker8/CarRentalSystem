using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Requests
{
    public class UpdateProfileRequest
    {
        [Required]
        [MinLength(3)]
        public string FullName { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }

        public string City { get; set; }
        public string Street { get; set; }
    }
}
