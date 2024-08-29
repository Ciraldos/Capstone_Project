namespace Capstone.Models.ViewModels
{
    public class MasterViewModel
    {
        public List<User> Users { get; set; }
        public List<Role> Roles { get; set; }
        public Role NewRole { get; set; }
    }
}
