using System.ComponentModel.DataAnnotations;

namespace Capstone.Models.ViewModels.Auth
{
    public class RegisterViewModel
    {
        public User? User { get; set; }

        [Required(ErrorMessage = "La conferma della password è obbligatoria.")]
        public required string ConfirmPassword { get; set; }
        public IFormFile? Img { get; set; }
        public List<int> SelectedGenres { get; set; } = [];

    }
}
