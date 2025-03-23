using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEVOLAplus.Intranet.Models.Licensing
{
    public class SoftwareLicense
    {
        [Key]
        public int SoftwareLicenseId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(50)]
        public required string ProductName { get; set; }

        [Required(ErrorMessage = "License key is required")]
        [MaxLength(100)]
        [Display(Name = "License Key")]
        public required string LicenseKey { get; set; }

        [Required]
        [Display(Name = "Valid Until")]
        public DateTime ValidUntil { get; set; }

        // Możesz tu dodać np. do którego pracownika przypisana
        // [ForeignKey("Employee")]
        // public int EmployeeId { get; set; }
        // public Employee? Employee { get; set; }
    }
}
