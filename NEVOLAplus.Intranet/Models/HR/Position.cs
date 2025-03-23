using System.ComponentModel.DataAnnotations;

namespace NEVOLAplus.Intranet.Models.HR
{
    public class Position
    {
        [Key]
        public int PositionId { get; set; }

        [Required(ErrorMessage = "Position name is required")]
        [MaxLength(50)]
        [Display(Name = "Position Name")]
        public required string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        // Kolekcja pracowników na danym stanowisku
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
