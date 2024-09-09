using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{

    public class CommentLike
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentLikeId { get; set; }

        public int CommentId { get; set; }
        [ForeignKey("CommentId")]
        public required Comment Comment { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public required User User { get; set; }
    }
}
