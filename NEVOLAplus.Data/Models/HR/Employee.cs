using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEVOLAplus.Data.Models.HR
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [MaxLength(30)]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(30)]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        // Relacja do Position
        [ForeignKey("Position")]
        public int PositionId { get; set; }
        public Position? Position { get; set; }
    }
}
