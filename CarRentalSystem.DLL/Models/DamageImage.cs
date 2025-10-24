using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Models
{
    public class DamageImage : BaseModel
    {
        [Required]
        public int DamageReportId { get; set; }
        public DamageReport DamageReport { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; }
    }
}
