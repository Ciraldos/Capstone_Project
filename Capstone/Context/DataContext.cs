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
        public virtual DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<EventTicketType> EventTicketType { get; set; }

        public DataContext(DbContextOptions<DataContext> opt) : base(opt)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CommentLike>()
                .HasOne(cl => cl.Comment)
                .WithMany(c => c.CommentLikes)
                .HasForeignKey(cl => cl.CommentId)
                .OnDelete(DeleteBehavior.Cascade); // A cascata

            // Se un utente viene eliminato, non eliminare a cascata i commenti o i like
            modelBuilder.Entity<CommentLike>()
                .HasOne(cl => cl.User)
                .WithMany()
                .HasForeignKey(cl => cl.UserId)
                .OnDelete(DeleteBehavior.Restrict); // No cascata

            //eventTicketType
            // Configure the many-to-many relationship using the EventTicketType entity
            modelBuilder.Entity<EventTicketType>()
                .HasKey(et => et.EventTicketTypeId); // Primary key for EventTicketType

            // Relationship between Event and EventTicketType
            modelBuilder.Entity<EventTicketType>()
                .HasOne(et => et.Event)
                .WithMany(e => e.EventTicketType)
                .HasForeignKey(et => et.EventId);

            // Relationship between TicketType and EventTicketType
            modelBuilder.Entity<EventTicketType>()
                .HasOne(et => et.TicketType)
                .WithMany(tt => tt.EventTicketType)
                .HasForeignKey(et => et.TicketTypeId);

            // Prevent EF from creating a new junction table
            modelBuilder.Entity<Event>()
                .HasMany(e => e.TicketTypes)
                .WithMany(tt => tt.Events)
                .UsingEntity<EventTicketType>(
                    j => j.HasOne(et => et.TicketType)
                          .WithMany(tt => tt.EventTicketType)
                          .HasForeignKey(et => et.TicketTypeId),
                    j => j.HasOne(et => et.Event)
                          .WithMany(e => e.EventTicketType)
                          .HasForeignKey(et => et.EventId)
                );
        }

    }
}
