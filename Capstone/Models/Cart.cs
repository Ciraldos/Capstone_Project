using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class Cart
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }

        //Foreign Key

        [Required]
        public int UserId { get; set; }

        //EF Reference 

        [ForeignKey("UserId")]
        public required User User { get; set; }

    }
}
