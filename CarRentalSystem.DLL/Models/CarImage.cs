using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Models
{
    public class CarImage : BaseModel
    {
        [Required]
        public int CarId { get; set; }
        public Car Car { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; }

        public bool IsMain { get; set; }

    }
}
