using EventEaseBooking.Data;
using EventEaseBooking.Models;
using EventEaseBooking.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace EventEaseBooking.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Index - List of events
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events.ToListAsync();
            return View(events);
        }

        // ✅ Create - GET
        public IActionResult Create() => View();

        // ✅ Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event eventObj, IFormFile imageFile, [FromServices] BlobStorageService blobService, [FromServices] IConfiguration config)
        {
            // If an image file is uploaded
            if (imageFile != null && imageFile.Length > 0)
            {
                var container = config["AzureStorage:EventContainer"];
                eventObj.ImageUrl = await blobService.UploadFileAsync(imageFile, container);
            }

            // If no image is provided, handle the case
            if (string.IsNullOrWhiteSpace(eventObj.ImageUrl))
            {
                ModelState.AddModelError("ImageUrl", "Please provide an image via upload or URL.");
            }

            if (ModelState.IsValid)
            {
                _context.Events.Add(eventObj);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(eventObj);  // Return the model back to the view if validation fails
        }

        // ✅ Details
        public async Task<IActionResult> Details(int id)
        {
            var eventObj = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eventObj == null) return NotFound();
            return View(eventObj);
        }

        // ✅ Edit - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null) return NotFound();
            return View(eventItem);
        }

        // ✅ Edit - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event updatedEvent)
        {
            if (id != updatedEvent.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(updatedEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(updatedEvent);
        }

        // ✅ Delete - GET
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var eventItem = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eventItem == null) return NotFound();
            return View(eventItem);
        }

        // ✅ Delete - POST (with bookings cleanup)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null) return NotFound();

            // ✅ Remove related bookings first
            if (eventItem.Bookings != null && eventItem.Bookings.Any())
            {
                _context.Bookings.RemoveRange(eventItem.Bookings);
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
