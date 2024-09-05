namespace Capstone.Models.ViewModels.Profile
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewEmail { get; set; }
        public List<Genre> AvailableGenres { get; set; }
        public List<int> SelectedGenreIds { get; set; }
    }
}
