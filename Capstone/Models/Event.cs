using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class Event
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        [StringLength(255)]
        public required string Description { get; set; }

        [Required]
        public required DateTime DateFrom { get; set; }

        [Required]
        public required DateTime DateTo { get; set; }

        [Required]
        public required int Quantity { get; set; }

        [Required]
        [StringLength(100)]
        public required string HostName { get; set; }

        //Foreign Key

        [Required]
        public int LocationId { get; set; }

        //EF reference
        [ForeignKey("LocationId")]
        [Required]
        public required Location Location { get; set; }

        public List<User> User { get; set; } = [];

        public List<Dj> Djs { get; set; } = [];

        public List<EventImg> EventImgs { get; set; } = [];
        public List<Comment> Comments { get; set; } = [];
        public List<TicketType> TicketTypes { get; set; } = [];


    }
}
