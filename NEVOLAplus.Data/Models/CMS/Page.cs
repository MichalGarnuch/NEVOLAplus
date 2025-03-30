using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEVOLAplus.Data.Models.CMS
{
    public class Page
    {
        [Key]
        public int PageId { get; set; }

        [Required(ErrorMessage = "Please enter a link title")]
        [MaxLength(20, ErrorMessage = "Link title must be max 20 chars")]
        [Display(Name = "Link title")]
        public required string LinkTitle { get; set; }

        [Required(ErrorMessage = "Please enter a page heading")]
        [MaxLength(50, ErrorMessage = "Heading must be max 50 chars")]
        [Display(Name = "Heading")]
        public required string Heading { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        [Display(Name = "Content")]
        public required string Content { get; set; }

        [Required(ErrorMessage = "Please specify display order")]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }
    }
}

