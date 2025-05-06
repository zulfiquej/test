using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ST10403582_CLDV6211_part1.Models;

namespace EventEaseBooking.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        public Event? Event { get; set; }

        public int? VenueId { get; set; } // ✅ Nullable

        public Venue? Venue { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string CustomerPhone { get; set; } = string.Empty;

        public DateTime BookingDate { get; set; } = DateTime.Now;
    }
}
