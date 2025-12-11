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

    public class ExtraassetsController : Controller
    {
        private readonly BookingDbContext _context;

        public ExtraassetsController(BookingDbContext context)
        {
            _context = context;
        }

        // GET: Extraassets
        public async Task<IActionResult> Index()
        {
            return View(await _context.Extraassets.ToListAsync());
        }

        // GET: Extraassets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var extraasset = await _context.Extraassets
                .FirstOrDefaultAsync(m => m.ExtraAssetId == id);
            if (extraasset == null)
            {
                return NotFound();
            }

            return View(extraasset);
        }

        // GET: Extraassets/Create
        public IActionResult Create()
        {
            ViewData["BodyClass"] = "admin-page";
            return View();
        }

        // POST: Extraassets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExtraAssetId,ExtraAssetName,AssetPrice,ExtraAssetStatus")] Extraasset extraasset)
        {
            ViewData["BodyClass"] = "admin-page";
            if (ModelState.IsValid)
            {
                _context.Add(extraasset);
                await _context.SaveChangesAsync();
                return RedirectToAction("Admins", "Admin");
            }
            return View(extraasset);
        }

        // GET: Extraassets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var extraasset = await _context.Extraassets.FindAsync(id);
            if (extraasset == null)
            {
                return NotFound();
            }
            return View(extraasset);
        }

        // POST: Extraassets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExtraAssetId,ExtraAssetName,AssetPrice,ExtraAssetStatus")] Extraasset extraasset)
        {
            if (id != extraasset.ExtraAssetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(extraasset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExtraassetExists(extraasset.ExtraAssetId))
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
            return View(extraasset);
        }

        // GET: Extraassets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var extraasset = await _context.Extraassets
                .FirstOrDefaultAsync(m => m.ExtraAssetId == id);
            if (extraasset == null)
            {
                return NotFound();
            }

            return View(extraasset);
        }

        // POST: Extraassets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var extraasset = await _context.Extraassets.FindAsync(id);
            if (extraasset != null)
            {
                _context.Extraassets.Remove(extraasset);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExtraassetExists(int id)
        {
            return _context.Extraassets.Any(e => e.ExtraAssetId == id);
        }
    }
}
