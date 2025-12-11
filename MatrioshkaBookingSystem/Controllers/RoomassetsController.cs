using MatrioshkaBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatrioshkaBookingSystem.Controllers
{


    public class RoomassetsController : Controller
    {
        private readonly BookingDbContext _context;

        public RoomassetsController(BookingDbContext context)
        {
            _context = context;
        }

        // GET: Roomassets
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roomassets.ToListAsync());
        }

        // GET: Roomassets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomasset = await _context.Roomassets
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (roomasset == null)
            {
                return NotFound();
            }

            return View(roomasset);
        }

        // GET: Roomassets/Create
        public IActionResult Create()
        {
            ViewData["BodyClass"] = "admin-page";
            return View();
        }

        // POST: Roomassets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssetName,AssetStatus")] Roomasset roomasset)
        {
            ViewData["BodyClass"] = "admin-page";

            if (ModelState.IsValid)
            {
                _context.Add(roomasset);
                await _context.SaveChangesAsync();

                return RedirectToAction("Admins", "Admin");
            }

            return View(roomasset);
        }

        // GET: Roomassets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomasset = await _context.Roomassets.FindAsync(id);
            if (roomasset == null)
            {
                return NotFound();
            }
            return View(roomasset);
        }

        // POST: Roomassets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssetId,AssetName,AssetStatus")] Roomasset roomasset)
        {
            if (id != roomasset.AssetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roomasset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomassetExists(roomasset.AssetId))
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
            return View(roomasset);
        }

        // GET: Roomassets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomasset = await _context.Roomassets
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (roomasset == null)
            {
                return NotFound();
            }

            return View(roomasset);
        }

        // POST: Roomassets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roomasset = await _context.Roomassets.FindAsync(id);
            if (roomasset != null)
            {
                _context.Roomassets.Remove(roomasset);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomassetExists(int id)
        {
            return _context.Roomassets.Any(e => e.AssetId == id);
        }
    }
}
