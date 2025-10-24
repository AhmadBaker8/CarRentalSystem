using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Requests
{
    public class AddReviewRequset
    {
        [Required]
        public int CarId { get; set; }
        [Required]
        public int BookingId { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]

        public int rating { get; set; }
        public string Comment { get; set; }

    }
}
