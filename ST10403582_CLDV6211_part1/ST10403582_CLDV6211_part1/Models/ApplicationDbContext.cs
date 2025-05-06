using Microsoft.EntityFrameworkCore;
using EventEaseBooking.Models;

namespace EventEaseBooking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Enforce unique event names
            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Name)
                .IsUnique();

            // Uncomment below if you reinstate venue-based event overlap constraints
            // modelBuilder.Entity<Event>()
            //     .HasIndex(e => new { e.VenueId, e.StartDate, e.EndDate })
            //     .IsUnique();
        }
    }
}
