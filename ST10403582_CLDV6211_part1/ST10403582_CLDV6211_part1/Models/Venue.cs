using System.ComponentModel.DataAnnotations;

namespace EventEaseBooking.Models
{
    public class Venue
    {
        //[Key]
        public int Id { get; set; }

        //[Required]
        //[StringLength(100)] // Maximum length of 100 characters
        public required string Name { get; set; } // Ensures it's required and not null

        //[Required]
        public required string Location { get; set; } // Required in EF

        //[Required]
        public int Capacity { get; set; } // Ensures capacity is required

        public string? Description { get; set; } // Nullable property
        public string? ImageUrl { get; set; } // Nullable property

        // Event relationship
        public List<Booking>? Bookings { get; set; } = new();// Nullable list for event relationships
    }
}
