using System.ComponentModel.DataAnnotations;

namespace NEVOLAplus.Data.Models.Inventory
{
    public class AssetType
    {
        [Key]
        public int AssetTypeId { get; set; }

        [Required(ErrorMessage = "Type name is required")]
        [MaxLength(50)]
        [Display(Name = "Type")]
        public required string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        // Kolekcja assets
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}
