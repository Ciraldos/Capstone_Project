using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class Role
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string RoleName { get; set; }

        // Riferimento alla tabella User
        public List<User> Users { get; set; } = [];
    }
}
