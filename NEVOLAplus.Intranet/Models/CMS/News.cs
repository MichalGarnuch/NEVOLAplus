using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEVOLAplus.Intranet.Models.CMS
{
    public class News
    {
        [Key]
        public int NewsId { get; set; }

        [Required(ErrorMessage = "Please enter a link title")]
        [MaxLength(20)]
        [Display(Name = "Link title")]
        public required string LinkTitle { get; set; }

        [Required(ErrorMessage = "Please enter a news title")]
        [MaxLength(50)]
        [Display(Name = "Title")]
        public required string Title { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        [Display(Name = "Content")]
        public required string Content { get; set; }

        [Required(ErrorMessage = "Please specify display order")]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }
    }
}
