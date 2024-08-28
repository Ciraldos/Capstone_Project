using Capstone.Models;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Context
{
    public class DataContext : DbContext
    {
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Dj> Djs { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventImg> EventImgs { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<ReviewImg> ReviewImgs { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<TicketType> TicketTypes { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public DataContext(DbContextOptions<DataContext> opt) : base(opt)
        {
        }

    }
}
