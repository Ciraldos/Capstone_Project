using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface ITicketTypeService
    {
        Task<TicketType> CreateAsync(TicketType ticketType);
        Task<TicketType> GetTicketTypeByIdAsync(int ticketTypeId);

        Task<List<TicketType>> GetAllTicketTypesAsync();
        Task<TicketType> UpdateAsync(TicketType ticketType);
        Task<bool> DeleteAsync(int ticketTypeId);

    }
}
