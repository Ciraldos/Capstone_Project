using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class Comment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Description { get; set; }

        [Required]
        public DateTime PublishedAt { get; set; } = DateTime.Now;

        [MaxLength(500)] // Puoi regolare la lunghezza se necessario
        public string? GifUrl { get; set; }

        //Foreign Keys

        [Required]
        public int EventId { get; set; }

        [Required]
        public int UserId { get; set; }
        public int? ParentCommentId { get; set; }


        // Riferimenti EF
        [ForeignKey("EventId")]
        public required Event Event { get; set; }

        [ForeignKey("UserId")]
        public required User User { get; set; }

        [ForeignKey("ParentCommentId")]
        public Comment? ParentComment { get; set; }
        public List<CommentLike> CommentLikes { get; set; } = [];
        public List<Comment> Replies { get; set; } = [];

    }
}
