using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Services
{
    public class TicketTypeService : ITicketTypeService
    {
        private readonly DataContext _ctx;
        public TicketTypeService(DataContext dataContext)
        {
            _ctx = dataContext;
        }

        public async Task<TicketType> CreateAsync(TicketType ticketType)
        {
            _ctx.TicketTypes.Add(ticketType);
            await _ctx.SaveChangesAsync();
            return ticketType;
        }

        public async Task<TicketType> GetTicketTypeByIdAsync(int ticketTypeId)
        {
            return await _ctx.TicketTypes.FindAsync(ticketTypeId);
        }

        public async Task<List<TicketType>> GetAllTicketTypesAsync()
        {
            return await _ctx.TicketTypes.ToListAsync();
        }

        public async Task<TicketType> UpdateAsync(TicketType ticketType)
        {
            _ctx.TicketTypes.Update(ticketType);
            await _ctx.SaveChangesAsync();
            return ticketType;
        }

        public async Task<bool> DeleteAsync(int ticketTypeId)
        {
            var ticketType = await _ctx.TicketTypes.FindAsync(ticketTypeId);
            if (ticketType == null)
            {
                return false;
            }
            _ctx.TicketTypes.Remove(ticketType);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
