using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEVOLAplus.Data.Models.CMS
{
    public class TextSnippet
    {
        [Key]
        public int TextSnippetId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Key")]
        public string Key { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;
    }
}
