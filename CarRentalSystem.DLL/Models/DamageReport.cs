using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Models
{
    public class DamageReport : BaseModel
    {
        [Required]
        public int BookingId { get; set; }
        public Booking Booking { get; set; }

        [Required]
        public string ReportedBy { get; set; }

        [Required]
        public DamageSeverity Severity { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        public decimal EstimatedRepairCost { get; set; }
        public decimal ActualRepairCost { get; set; }

        public bool IsCustomerResponsible { get; set; }

        public DamageStatus Status { get; set; } = DamageStatus.Reported;

        // Images
        public List<DamageImage> DamageImages { get; set; } = new();
    }

    public enum DamageSeverity
    {
        Minor = 1,
        Moderate = 2,
        Severe = 3,
        TotalLoss = 4
    }

    public enum DamageStatus
    {
        Reported = 1,
        UnderReview = 2,
        Approved = 3,
        Repaired = 4,
        Closed = 5
    }
}
