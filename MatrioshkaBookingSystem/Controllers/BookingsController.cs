using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MatrioshkaBookingSystem.Models;
using System.Security.Claims;

namespace MatrioshkaBookingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly BookingDbContext _context;

        public BookingsController(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bookingDbContext = _context.Bookings.Include(b => b.Billing).Include(b => b.Room).Include(b => b.User);
            return View(await bookingDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Billing)
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        private IActionResult ReturnViewWithViewData(Booking booking)
        {
            ViewData["BillingId"] = new SelectList(_context.Billinginfos, "BillingId", "BillingId", booking.BillingId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", booking.RoomId);
            ViewBag.Hotels = new SelectList(_context.Hotels.ToList(), "HotelId", "HotelName");
            ViewBag.ExtraAssets = _context.Extraassets
                 .Where(ea => ea.ExtraAssetStatus == "Available").ToList();

            return View(booking);
        }

        public IActionResult Create()
        {
            ViewBag.Hotels = new SelectList(_context.Hotels, "HotelId", "HotelName");
            ViewData["BillingId"] = new SelectList(_context.Billinginfos, "BillingId", "BillingId");
            ViewData["RoomId"] = new SelectList(Enumerable.Empty<SelectListItem>(), "RoomId", "RoomId");
            ViewBag.ExtraAssets = _context.Extraassets
                .Where(ea => ea.ExtraAssetStatus == "Available").ToList();

            return View(new Booking());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,RoomId,BillingId,DateofBooking,EndofBooking")] Booking booking, int[] SelectedExtraAssets)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError(string.Empty, "Oh-Uh: User not identified!");
                return ReturnViewWithViewData(booking);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Oh-Uh: User not identified!");
                return ReturnViewWithViewData(booking);
            }

            booking.UserId = user.UserId;

            var conflict = await _context.Bookings
                .Where(b => b.RoomId == booking.RoomId)
                .Where(b => booking.DateofBooking < b.EndofBooking && booking.EndofBooking > b.DateofBooking)
                .AnyAsync();

            if(conflict)
            {
                ModelState.AddModelError(string.Empty, "This room is already taken!, Pick another one.");
                return ReturnViewWithViewData(booking);

            }

            if (booking.Billing != null)
            {
                booking.Billing.UserId = user.UserId;
                _context.Billinginfos.Add(booking.Billing);

                await _context.SaveChangesAsync();
                booking.BillingId = booking.Billing.BillingId; 
            }
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();

                if (SelectedExtraAssets != null && SelectedExtraAssets.Length > 0)
                {
                    var assets = _context.Extraassets
                        .Where(ea => SelectedExtraAssets.Contains(ea.ExtraAssetId)).ToList();

                    foreach (var asset in assets)
                    {
                        var bookingExtraAsset = new Bookingextraasset
                        {
                            BookingId = booking.BookingId,
                            ExtraAssetId = asset.ExtraAssetId,
                            ExtraAssetPrice = asset.AssetPrice

                        };
                        _context.Bookingextraassets.Add(bookingExtraAsset);
                    }
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Admins", "Admin");
            }

            return ReturnViewWithViewData(booking);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["BillingId"] = new SelectList(_context.Billinginfos, "BillingId", "BillingId", booking.BillingId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", booking.RoomId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", booking.UserId);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,RoomId,BillingId,DateofBooking,EndofBooking")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);

            if (user != null)
            {
                booking.UserId = user.UserId;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Admins", "Admin");
            }
            ViewData["BillingId"] = new SelectList(_context.Billinginfos, "BillingId", "BillingId", booking.BillingId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", booking.RoomId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", booking.UserId);
            return View(booking);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Billing)
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}