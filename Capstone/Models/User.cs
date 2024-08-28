using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(PhoneNum), IsUnique = true)]
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Username { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Password)]
        [PersonalData]
        public required string PasswordHash { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        [StringLength(50)]
        public required string Surname { get; set; }

        [Required]
        public required DateTime BirthDate { get; set; }

        [Required]
        [StringLength(15)]
        public required string Gender { get; set; }

        [Required]
        [StringLength(100)]
        public required string Country { get; set; }

        [Required]
        [StringLength(100)]
        public required string City { get; set; }

        [Required]
        [StringLength(20)]
        public required string PostalCode { get; set; }

        [Required]
        [StringLength(20)]
        public required string PhoneNum { get; set; }

        public byte[]? Img { get; set; }

        // EF References
        public List<Role> Roles { get; set; } = [];
        public List<Event> Events { get; set; } = [];
        public List<Genre> Genres { get; set; } = [];
    }
}
