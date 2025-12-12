using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MatrioshkaBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace MatrioshkaBookingSystem.Controllers
{
    // Only admins can edit hotels
    [Authorize(Roles = "Admin")]
    public class HotelsController : Controller
    {
        // Controller 
        private readonly BookingDbContext _context;
        private readonly IWebHostEnvironment _env;

        // Constructor
        public HotelsController(BookingDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Index get
        public async Task<IActionResult> Index()
        {
            return View(await _context.Hotels.ToListAsync());
        }

        // Details get
        public async Task<IActionResult> Details(int? id)
        {

            // Validation
            if (id == null)
                return NotFound();

            var hotel = await _context.Hotels.FirstOrDefaultAsync(m => m.HotelId == id);
            if (hotel == null)
                return NotFound();

            return View(hotel);
        }

        // Create 
        public IActionResult Create()
        {
            ViewData["BodyClass"] = "admin-page";
            return View();
        }

        // Create post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HotelId,HotelName,HotelLocation,HotelStatus")] Hotel hotel, IFormFile HotelImage)
        {
            // Validation
            if (ModelState.IsValid)
            {
                if (HotelImage != null && HotelImage.Length > 0)
                {
                    string folderPath = Path.Combine(_env.WebRootPath, "img", "Hotels");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(HotelImage.FileName);
                    string filepath = Path.Combine(folderPath, filename);

                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        await HotelImage.CopyToAsync(stream);
                    }

                    // For images
                    hotel.ImagePath = "/img/Hotels/" + filename;
                }

                _context.Add(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction("Admins", "Admin");
            }

            return View(hotel);
        }

        // Edit get
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                return NotFound();

            return View(hotel);
        }

        // Edit ppost
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, Hotel hotel, IFormFile ImageFile)
        {
            // Validation:
            var hotelToUpdate = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == id);

            if (hotelToUpdate == null)
                return NotFound();

            hotelToUpdate.HotelName = hotel.HotelName;
            hotelToUpdate.HotelLocation = hotel.HotelLocation;
            hotelToUpdate.HotelStatus = hotel.HotelStatus;

            // Validation for images:
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string folderPath = Path.Combine(_env.WebRootPath, "img", "Hotels");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filename = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string filepath = Path.Combine(folderPath, filename);

                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                hotelToUpdate.ImagePath = "/img/Hotels/" + filename;
            }
            // Saving changes
            try
            {
                _context.Update(hotelToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(hotelToUpdate);
            }
        }

        // Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var hotel = await _context.Hotels.FirstOrDefaultAsync(m => m.HotelId == id);
            if (hotel == null)
                return NotFound();

            return View(hotel);
        }
        //delete post
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel != null)
            {
                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Admins", "Admin");
        }

        // Validating if hotel exists
        private bool HotelExists(int id)
        {
            return _context.Hotels.Any(e => e.HotelId == id);
        }
    }
}
