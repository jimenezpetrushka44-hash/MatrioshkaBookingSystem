using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MatrioshkaBookingSystem.Models;

namespace MatrioshkaBookingSystem.Controllers
{
    public class HotelsController : Controller
    {
        private readonly BookingDbContext _context;
        private readonly IWebHostEnvironment _env;

        public HotelsController(BookingDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Hotels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Hotels.ToListAsync());
        }

        // GET: Hotels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(m => m.HotelId == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // GET: Hotels/Create
        public IActionResult Create()
        {
            ViewData["BodyClass"] = "admin-page";
            return View();
        }

        // POST: Hotels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HotelId,HotelName,HotelLocation,HotelStatus")] Hotel hotel, IFormFile HotelImage)
        {
            if (ModelState.IsValid)
            {
                // Adding Image file handling:
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

        // GET: Hotels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel);
        }

        // POST: Hotels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HotelId,HotelName,HotelLocation,HotelStatus")] Hotel hotel)
        {
            if (id != hotel.HotelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hotel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotel.HotelId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }

        // GET: Hotels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(m => m.HotelId == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // POST: Hotels/Delete/5
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
            _context.Database.ExecuteSqlRaw(@"
        SET @count = 0;
        UPDATE Hotels SET HotelId = @count:=@count+1;
    ");

            _context.Database.ExecuteSqlRaw("ALTER TABLE Hotels AUTO_INCREMENT = 1;");
            return RedirectToAction("Admins","Admin");

        }


        private bool HotelExists(int id)
        {
            return _context.Hotels.Any(e => e.HotelId == id);
        }
    }
}
