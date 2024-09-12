using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class Genre
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GenreId { get; set; }
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        //EF Reference
        public List<User> Users { get; set; } = [];
        public List<Event> Events { get; set; } = [];


    }
}