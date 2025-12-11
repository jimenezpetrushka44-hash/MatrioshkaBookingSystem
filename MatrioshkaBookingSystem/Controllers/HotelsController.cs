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
    [Authorize(Roles = "Admin")]
    public class HotelsController : Controller
    {
        private readonly BookingDbContext _context;
        private readonly IWebHostEnvironment _env;

        public HotelsController(BookingDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Hotels.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var hotel = await _context.Hotels.FirstOrDefaultAsync(m => m.HotelId == id);
            if (hotel == null)
                return NotFound();

            return View(hotel);
        }

        public IActionResult Create()
        {
            ViewData["BodyClass"] = "admin-page";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HotelId,HotelName,HotelLocation,HotelStatus")] Hotel hotel, IFormFile HotelImage)
        {
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

                    hotel.ImagePath = "/img/Hotels/" + filename;
                }

                _context.Add(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction("Admins", "Admin");
            }

            return View(hotel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                return NotFound();

            return View(hotel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, Hotel hotel, IFormFile ImageFile)
        {
            var hotelToUpdate = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == id);

            if (hotelToUpdate == null)
                return NotFound();

            hotelToUpdate.HotelName = hotel.HotelName;
            hotelToUpdate.HotelLocation = hotel.HotelLocation;
            hotelToUpdate.HotelStatus = hotel.HotelStatus;

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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var hotel = await _context.Hotels.FirstOrDefaultAsync(m => m.HotelId == id);
            if (hotel == null)
                return NotFound();

            return View(hotel);
        }

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

        private bool HotelExists(int id)
        {
            return _context.Hotels.Any(e => e.HotelId == id);
        }
    }
}
