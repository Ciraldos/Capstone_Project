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

        [Required(ErrorMessage = "Il nome utente è obbligatorio.")]
        [StringLength(50, ErrorMessage = "Il nome utente non può superare i 50 caratteri.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "L'email è obbligatoria.")]
        [StringLength(50, ErrorMessage = "L'email non può superare i 50 caratteri.")]
        [EmailAddress(ErrorMessage = "Inserisci un'email valida.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "La password è obbligatoria.")]
        [StringLength(255, ErrorMessage = "La password non può superare i 255 caratteri.")]
        [DataType(DataType.Password, ErrorMessage = "Formato password non valido.")]
        [PersonalData]
        public required string PasswordHash { get; set; }

        [Required(ErrorMessage = "Il nome è obbligatorio.")]
        [StringLength(50, ErrorMessage = "Il nome non può superare i 50 caratteri.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Il cognome è obbligatorio.")]
        [StringLength(50, ErrorMessage = "Il cognome non può superare i 50 caratteri.")]
        public required string Surname { get; set; }

        [Required(ErrorMessage = "La data di nascita è obbligatoria.")]
        public required DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Il genere è obbligatorio.")]
        [StringLength(15, ErrorMessage = "Il genere non può superare i 15 caratteri.")]
        [RegularExpression("^(uomo|donna|altro)$", ErrorMessage = "Il genere deve essere uno tra: uomo, donna, altro.")]
        public required string Gender { get; set; }

        [Required(ErrorMessage = "Il paese è obbligatorio.")]
        [StringLength(100, ErrorMessage = "Il paese non può superare i 100 caratteri.")]
        public required string Country { get; set; }

        [Required(ErrorMessage = "La città è obbligatoria.")]
        [StringLength(100, ErrorMessage = "La città non può superare i 100 caratteri.")]
        public required string City { get; set; }

        [Required(ErrorMessage = "Il codice postale è obbligatorio.")]
        [StringLength(20, ErrorMessage = "Il codice postale non può superare i 20 caratteri.")]
        public required string PostalCode { get; set; }

        [Required(ErrorMessage = "Il numero di telefono è obbligatorio.")]
        [StringLength(20, ErrorMessage = "Il numero di telefono non può superare i 20 caratteri.")]
        [RegularExpression(@"^\+?[0-9]*$", ErrorMessage = "Il numero di telefono può contenere solo numeri e il simbolo '+'.")]
        public required string PhoneNum { get; set; }

        public byte[]? Img { get; set; }

        // EF References
        public List<Role> Roles { get; set; } = [];
        public List<Event> Events { get; set; } = [];
        public List<Genre> Genres { get; set; } = [];
    }
}
