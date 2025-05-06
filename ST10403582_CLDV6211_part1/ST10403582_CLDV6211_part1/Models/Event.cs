using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEaseBooking.Models
{
    public class Event
    {
        //[Key]
        public int Id { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "Name must be less than 100 characters.")]
        public string Name { get; set; } = string.Empty; // Default value avoids null issues

        //[Required]
        public DateTime StartDate { get; set; }

        //[Required]
        public DateTime EndDate { get; set; }

        //[Required]
        // public int VenueId { get; set; } // Foreign key

        public string? Description { get; set; } // Optional

        //[Url]
        public string? ImageUrl { get; set; } // Optional

        public List<Booking>? Bookings { get; set; } = new();
    }
}
