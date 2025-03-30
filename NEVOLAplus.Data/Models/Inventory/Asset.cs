using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEVOLAplus.Data.Models.Inventory
{
    public class Asset
    {
        [Key]
        public int AssetId { get; set; }

        [Required(ErrorMessage = "Please enter an asset name")]
        [MaxLength(50)]
        public required string Name { get; set; }

        [Required]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Cost is required")]
        [Column(TypeName = "money")]
        public decimal Cost { get; set; }

        // Relacja do AssetType
        [ForeignKey("AssetType")]
        public int AssetTypeId { get; set; }
        public AssetType? AssetType { get; set; }
    }
}
