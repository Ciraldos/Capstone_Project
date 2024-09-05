using System.ComponentModel.DataAnnotations;

namespace Capstone.Models.ViewModels.Profile
{
    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "La vecchia password è obbligatoria.")]
        [DataType(DataType.Password)]
        public required string OldPassword { get; set; }

        [Required(ErrorMessage = "La nuova password è obbligatoria.")]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }
    }
}
