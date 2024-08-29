using System.ComponentModel.DataAnnotations;

namespace Capstone.Models.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Il nome utente è obbligatorio.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "La password è obbligatoria.")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
