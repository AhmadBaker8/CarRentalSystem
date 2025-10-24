using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Requests
{
    public class MarkAsPickedUpRequest
    {
        [Required]
        [Range(9,int.MaxValue)]
        public int PickupMileage { get; set; }
    }
}
