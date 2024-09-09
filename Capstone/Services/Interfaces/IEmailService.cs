using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, List<Ticket> tickets);
    }
}
