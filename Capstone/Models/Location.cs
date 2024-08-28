using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class Location
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LocationId { get; set; }

        [Required]
        [MaxLength(255)]
        public required string LocationName { get; set; }

        [Required]
        [MaxLength(255)]
        public required string AddressGoogleApi { get; set; }
    }
}
