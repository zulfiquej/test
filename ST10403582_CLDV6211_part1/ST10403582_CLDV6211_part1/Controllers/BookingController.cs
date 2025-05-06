using EventEaseBooking.Data;
using EventEaseBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventEaseBooking.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ View All Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .ToListAsync();

            return View(bookings);
        }

        // ✅ View Booking Details
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();
            return View(booking);
        }

        // ✅ Create Booking - GET
        public IActionResult Create()
        {
            ViewBag.Venues = _context.Venues.ToList();
            return View();
        }

        // ✅ Create Booking - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking, string EventName)
        {
            if (string.IsNullOrWhiteSpace(EventName))
            {
                ViewBag.EventError = "Event name is required.";
                ViewBag.Venues = _context.Venues.ToList();
                return View(booking);
            }

            var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.Name == EventName);

            if (existingEvent == null)
            {
                var newEvent = new Event
                {
                    Name = EventName,
                    Description = "N/A",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1),
                    ImageUrl = "https://via.placeholder.com/300x200.png?text=Event+Image"
                };

                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();
                booking.EventId = newEvent.Id;
            }
            else
            {
                booking.EventId = existingEvent.Id;
            }

            // ✅ Double booking prevention (checks based on venue selection)
            if (booking.VenueId.HasValue && booking.VenueId.Value != 0)
            {
                bool doubleBooked = await _context.Bookings.AnyAsync(b =>
                    b.VenueId == booking.VenueId &&
                    b.BookingDate.Date == booking.BookingDate.Date);

                if (doubleBooked)
                {
                    ModelState.AddModelError("", "A booking already exists for this venue on the selected date.");
                    ViewBag.Venues = _context.Venues.ToList();
                    return View(booking);
                }
            }
            else
            {
                bool doubleBooked = await _context.Bookings.AnyAsync(b =>
                    b.BookingDate.Date == booking.BookingDate.Date && b.VenueId != null);

                if (doubleBooked)
                {
                    ModelState.AddModelError("", "A booking already exists on the selected date, regardless of venue.");
                    ViewBag.Venues = _context.Venues.ToList();
                    return View(booking);
                }
            }

            // ✅ If no venue selected, set VenueId to null
            if (booking.VenueId.HasValue && booking.VenueId.Value == 0)
            {
                booking.VenueId = null;
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ✅ Edit Booking - GET
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Event)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            ViewBag.EventName = booking.Event?.Name;
            ViewBag.Venues = await _context.Venues.ToListAsync();
            return View(booking);
        }

        // ✅ Edit Booking - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking updatedBooking, string EventName)
        {
            if (id != updatedBooking.Id) return BadRequest();

            var existingBooking = await _context.Bookings
                .Include(b => b.Event)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBooking == null) return NotFound();

            if (string.IsNullOrWhiteSpace(EventName))
            {
                ViewBag.EventError = "Event name is required.";
                ViewBag.Venues = await _context.Venues.ToListAsync();
                return View(updatedBooking);
            }

            var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.Name == EventName);
            if (existingEvent == null)
            {
                var newEvent = new Event
                {
                    Name = EventName,
                    Description = "N/A",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1),
                    ImageUrl = "https://via.placeholder.com/300x200.png?text=Event+Image"
                };

                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();
                existingBooking.EventId = newEvent.Id;
            }
            else
            {
                existingBooking.EventId = existingEvent.Id;
            }

            // ✅ Double booking prevention (checks based on venue selection)
            if (updatedBooking.VenueId.HasValue && updatedBooking.VenueId.Value != 0)
            {
                bool doubleBooked = await _context.Bookings.AnyAsync(b =>
                    b.Id != id &&
                    b.VenueId == updatedBooking.VenueId &&
                    b.BookingDate.Date == updatedBooking.BookingDate.Date);

                if (doubleBooked)
                {
                    ModelState.AddModelError("", "Another booking already exists for this venue on the selected date.");
                    ViewBag.EventName = EventName;
                    ViewBag.Venues = await _context.Venues.ToListAsync();
                    return View(updatedBooking);
                }
            }
            else
            {
                bool doubleBooked = await _context.Bookings.AnyAsync(b =>
                    b.BookingDate.Date == updatedBooking.BookingDate.Date &&
                    b.VenueId != null && b.Id != id);

                if (doubleBooked)
                {
                    ModelState.AddModelError("", "Another booking already exists on the selected date, regardless of venue.");
                    ViewBag.EventName = EventName;
                    ViewBag.Venues = await _context.Venues.ToListAsync();
                    return View(updatedBooking);
                }
            }

            // ✅ If no venue selected, set VenueId to null
            if (updatedBooking.VenueId.HasValue && updatedBooking.VenueId.Value == 0)
            {
                updatedBooking.VenueId = null;
            }

            existingBooking.CustomerName = updatedBooking.CustomerName;
            existingBooking.CustomerEmail = updatedBooking.CustomerEmail;
            existingBooking.CustomerPhone = updatedBooking.CustomerPhone;
            existingBooking.BookingDate = updatedBooking.BookingDate;
            existingBooking.VenueId = updatedBooking.VenueId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ✅ Delete Booking - GET
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Event)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();
            return View(booking);
        }

        // ✅ Delete Booking - POST
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
