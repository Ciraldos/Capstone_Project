using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class EventImg
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventImgId { get; set; }

        [Required]
        public required byte[] ImgData { get; set; }

        // Foreign Key
        [Required]
        public int EventId { get; set; }

        // Riferimento EF
        [ForeignKey("EventId")]
        public required Event Event { get; set; }
    }
}
