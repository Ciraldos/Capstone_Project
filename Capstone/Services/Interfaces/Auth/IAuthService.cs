using Capstone.Models;
using Capstone.Models.ViewModels.Auth;

namespace Capstone.Services.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterViewModel user);
        Task<User> LoginAsync(LoginViewModel user);
    }
}
