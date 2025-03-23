using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NEVOLAplus.Intranet.Models.Inventory;  // Jeśli chcesz rezerwować Asset
using NEVOLAplus.Intranet.Models.HR;        // Jeśli rezerwuje Employee

namespace NEVOLAplus.Intranet.Models.Reservation
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [MaxLength(30)]
        public required string Status { get; set; }

        // Relacja do Asset (co jest rezerwowane)
        [ForeignKey("Asset")]
        public int AssetId { get; set; }
        public Asset? Asset { get; set; }

        // Relacja do Employee (kto rezerwuje)
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
