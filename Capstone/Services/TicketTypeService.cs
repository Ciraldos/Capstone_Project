using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Services
{
    public class TicketTypeService : ITicketTypeService
    {
        private readonly DataContext _dataContext;
        public TicketTypeService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<TicketType> CreateAsync(TicketType ticketType)
        {
            _dataContext.TicketTypes.Add(ticketType);
            await _dataContext.SaveChangesAsync();
            return ticketType;
        }

        public async Task<TicketType> GetTicketTypeByIdAsync(int ticketTypeId)
        {
            return await _dataContext.TicketTypes.FindAsync(ticketTypeId);
        }

        public async Task<List<TicketType>> GetAllTicketTypesAsync()
        {
            return await _dataContext.TicketTypes.ToListAsync();
        }

        public async Task<TicketType> UpdateAsync(TicketType ticketType)
        {
            _dataContext.TicketTypes.Update(ticketType);
            await _dataContext.SaveChangesAsync();
            return ticketType;
        }

        public async Task<bool> DeleteAsync(int ticketTypeId)
        {
            var ticketType = await _dataContext.TicketTypes.FindAsync(ticketTypeId);
            if (ticketType == null)
            {
                return false;
            }
            _dataContext.TicketTypes.Remove(ticketType);
            await _dataContext.SaveChangesAsync();
            return true;
        }
    }
}
