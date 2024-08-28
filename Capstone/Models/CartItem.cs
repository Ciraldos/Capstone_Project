using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class CartItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemId { get; set; }

        [Required]
        public required int Quantity { get; set; }

        //Foreign Keys

        [Required]
        public int CartId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int TicketTypeId { get; set; }

        // Riferimenti EF

        [ForeignKey("CartId")]
        public required Cart Cart { get; set; }


        [ForeignKey("EventId")]
        public required Event Event { get; set; }

        [ForeignKey("TicketTypeId")]
        public required TicketType TicketType { get; set; }
    }
}
