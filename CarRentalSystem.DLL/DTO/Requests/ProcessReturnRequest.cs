using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Requests
{
    public class ProcessReturnRequest
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int ReturnMileage { get; set; }
        public string? ReturnConditionNotes { get; set; }
    }
}
