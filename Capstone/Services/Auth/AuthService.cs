using Capstone.Context;
using Capstone.Helpers;
using Capstone.Models;
using Capstone.Models.ViewModels.Auth;
using Capstone.Services.Interfaces.Auth;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _ctx;
        private readonly IPasswordHelper _passwordHelper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(DataContext dataContext, IPasswordHelper passwordHelper, ILogger<AuthService> logger)
        {
            _ctx = dataContext;
            _passwordHelper = passwordHelper;
            _logger = logger;
        }

        public async Task<User> RegisterAsync(RegisterViewModel model)
        {
            if (model.User!.PasswordHash != model.ConfirmPassword)
            {
                _logger.LogWarning("Le password non corrispondono.");
                throw new Exception("Le password non corrispondono.");
            }

            try
            {
                var existingUser = await _ctx.Users
                    .Where(u => u.Username == model.User.Username || u.Email == model.User.Email || u.PhoneNum == model.User.PhoneNum)
                    .FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    if (existingUser.Username == model.User.Username)
                    {
                        _logger.LogWarning("Tentativo di registrazione con username già in uso: {Username}", model.User.Username);
                        throw new Exception("Il nome utente è già in uso.");
                    }
                    if (existingUser.Email == model.User.Email)
                    {
                        _logger.LogWarning("Tentativo di registrazione con email già in uso: {Email}", model.User.Email);
                        throw new Exception("L'indirizzo email è già in uso.");
                    }
                    if (existingUser.PhoneNum == model.User.PhoneNum)
                    {
                        _logger.LogWarning("Tentativo di registrazione con numero di telefono già in uso: {PhoneNum}", model.User.PhoneNum);
                        throw new Exception("Il numero di telefono è già in uso.");
                    }
                }

                byte[]? imageBytes = null;
                if (model.Img != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.Img.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }
                var selectedGenres = await _ctx.Genres
                    .Where(g => model.SelectedGenres.Contains(g.GenreId))
                    .ToListAsync();

                var userRegister = new User
                {
                    Username = model.User.Username,
                    Email = model.User.Email,
                    PasswordHash = string.Empty,
                    Name = model.User.Name,
                    Surname = model.User.Surname,
                    BirthDate = model.User.BirthDate,
                    Gender = model.User.Gender,
                    Country = model.User.Country,
                    City = model.User.City,
                    PostalCode = model.User.PostalCode,
                    PhoneNum = model.User.PhoneNum,
                    Img = imageBytes,
                    Genres = selectedGenres,
                };

                userRegister.PasswordHash = _passwordHelper.HashPassword(model.User.PasswordHash);

                var userRole = await _ctx.Roles.Where(r => r.RoleId == 1).FirstOrDefaultAsync();

                userRegister.Roles.Add(userRole!);

                await _ctx.Users.AddAsync(userRegister);
                await _ctx.SaveChangesAsync();

                _logger.LogInformation("Registrazione completata con successo per l'username: {Username}", model.User.Username);
                return userRegister;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la registrazione per l'username: {Username}", model.User.Username);
                throw;
            }
        }

        public async Task<User> LoginAsync(LoginViewModel model)
        {
            try
            {
                var existingUser = await _ctx.Users
                    .Include(u => u.Roles)
                    .Where(u => u.Username == model.Username)
                    .FirstOrDefaultAsync();

                if (existingUser == null)
                {
                    _logger.LogWarning("Tentativo di login fallito: username {Username} non trovato.", model.Username);
                    throw new Exception("Il nome utente non esiste.");
                }

                if (!_passwordHelper.VerifyPassword(model.Password, existingUser.PasswordHash))
                {
                    _logger.LogWarning("Tentativo di login fallito: password errata per l'username {Username}.", model.Username);
                    throw new Exception("Password errata.");
                }

                _logger.LogInformation("Login riuscito per l'username: {Username}", model.Username);
                return existingUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il login per l'username: {Username}", model.Username);
                throw;
            }
        }
    }
}
