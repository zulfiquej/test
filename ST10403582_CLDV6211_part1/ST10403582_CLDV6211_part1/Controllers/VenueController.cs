using EventEaseBooking.Data;
using EventEaseBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace EventEaseBooking.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VenueController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var venues = await _context.Venues.ToListAsync();
            return View(venues);
        }

        public async Task<IActionResult> Details(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
                return NotFound();
            return View(venue);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Events = await _context.Events
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                })
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                _context.Venues.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Events = await _context.Events
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                })
                .ToListAsync();

            return View(venue);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
                return NotFound();
            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (id != venue.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(venue);
        }

        // ✅ Delete - GET
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // ✅ Delete - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
