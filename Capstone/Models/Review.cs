using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class Review
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewId { get; set; }

        [Required]
        public required int Rating { get; set; }

        [Required]
        [StringLength(100)]
        public required string Title { get; set; }

        [Required]
        [StringLength(255)]
        public required string Description { get; set; }

        [Required]
        public required DateTime ReviewDate { get; set; }

        // Foreign Key 
        public int EventId { get; set; }
        public int UserId { get; set; }

        // EF Reference
        [ForeignKey("EventId")]
        [Required]
        public required Event Event { get; set; }

        [ForeignKey("UserId")]
        [Required]
        public required User User { get; set; }
    }
}
