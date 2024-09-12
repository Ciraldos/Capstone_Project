using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class ReviewImg
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewImgId { get; set; }

        [Required]
        public string? FilePath { get; set; }

        // Foreign Key 
        public int ReviewId { get; set; }

        //EF Reference
        [ForeignKey("ReviewId")]
        [Required]
        public required Review Review { get; set; }

    }
}
