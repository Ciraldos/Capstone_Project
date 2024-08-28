using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class TicketType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public required string TicketTypeName { get; set; }

        [StringLength(255)]
        public string? TicketTypeDescription { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Price { get; set; }
    }
}
