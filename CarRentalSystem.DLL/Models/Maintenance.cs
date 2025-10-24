using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Models
{
    public class Maintenance : BaseModel
    {
        [Required]
        public int CarId { get; set; }
        public Car Car { get; set; }

        [Required]
        public MaintenanceType Type { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        public decimal Cost { get; set; }

        public int MileageAtMaintenance { get; set; }

        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;

        [MaxLength(200)]
        public string? PerformedBy { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public enum MaintenanceType
    {
        Regular = 1,
        OilChange = 2,
        TireReplacement = 3,
        BrakeService = 4,
        EngineRepair = 5,
        BodyWork = 6,
        Other = 7
    }

    public enum MaintenanceStatus
    {
        Scheduled = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4
    }
}
