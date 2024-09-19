using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class Dj
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DjId { get; set; }

        [Required]
        [StringLength(100)]
        public required string ArtistName { get; set; }

        [Required]
        public string Img { get; set; }

        [Required]
        [StringLength(255)]
        public required string ArtistSpotifyId { get; set; }

        public string? ArtistDescription { get; set; }


        // EF reference

        public List<Event> Events { get; set; } = [];
    }
}