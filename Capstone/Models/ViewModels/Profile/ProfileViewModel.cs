using System.ComponentModel.DataAnnotations;

namespace Capstone.Models.ViewModels.Profile
{
    public class ProfileViewModel
    {
        public User User { get; set; }

        [Required(ErrorMessage = "La vecchia password è obbligatoria.")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "La vecchia password è obbligatoria.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "La nuova email è obbligatoria.")]
        [EmailAddress(ErrorMessage = "Inserisci un'email valida.")]
        public string NewEmail { get; set; }
        public List<Genre> AvailableGenres { get; set; }
        public List<int> SelectedGenreIds { get; set; }
    }
}
