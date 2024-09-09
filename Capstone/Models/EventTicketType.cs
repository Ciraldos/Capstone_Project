using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class EventTicketType
    {
        [Key]
        public int EventTicketTypeId { get; set; }

        // Foreign Key to Event
        [Required]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }

        // Foreign Key to TicketType
        [Required]
        public int TicketTypeId { get; set; }

        [ForeignKey("TicketTypeId")]
        public TicketType TicketType { get; set; }

        // Quantity of available tickets for this event and ticket type
        [Required]
        public int AvailableQuantity { get; set; }
    }
}
