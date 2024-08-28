using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class Ticket
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketId { get; set; }

        [Required]
        public required DateTime PurchaseDate { get; set; }

        [Required]
        [StringLength(50)]
        public required string NumTicket { get; set; }

        // Foreign Keys
        public int TicketTypeId { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }

        // EF Reference
        [ForeignKey("TicketTypeId")]
        public required TicketType TicketType { get; set; }

        [ForeignKey("UserId")]
        public required User User { get; set; }

        [ForeignKey("EventId")]
        public required Event Event { get; set; }
    }
}
