using System.ComponentModel.DataAnnotations;

namespace Capstone.Models.ViewModels
{
    public class ChangeEmailViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "La nuova email è obbligatoria.")]
        [EmailAddress(ErrorMessage = "Inserisci un'email valida.")]
        public required string NewEmail { get; set; }
    }
}
