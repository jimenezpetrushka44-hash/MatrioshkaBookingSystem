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

        public IActionResult Create(int hotelId)
        {
            var hotel = _context.Hotels.Find(hotelId);

            if (hotel == null)
            {
                return NotFound();
            }

            ViewBag.SelectedHotelName = hotel.HotelName;
            ViewBag.SelectedHotelId = hotelId;
            ViewData["BillingId"] = new SelectList(_context.Billinginfos, "BillingId", "BillingId");
            ViewBag.ExtraAssets = _context.Extraassets
                .Where(ea => ea.ExtraAssetStatus == "Available").ToList();

            var availableRooms = _context.Rooms
                .Include(r => r.Type)
                .Include(r => r.Floor)
                .Where(r => r.Floor.HotelId == hotelId && r.RoomStatus == "Available")
                .ToList();

            if (availableRooms.Any())
            {
                ViewData["RoomId"] = new SelectList(availableRooms.Select(r => new
                {
                    RoomId = r.RoomId,
                    RoomDisplay = $"Room: {r.RoomId} {r.Type.TypeName} - $ {r.Type.TypePrice:F2}"
                }), "RoomId", "RoomDisplay");

            }
            else
            {
                ViewData["RoomId"] = new SelectList(Enumerable.Empty<SelectListItem>(), "RoomId", "RoomId");
            }

            return View(new Booking());
        }

        public async Task<IActionResult> Invoice(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.Floor)
                        .ThenInclude(f => f.Hotel)
                .Include(b => b.Room)
                    .ThenInclude(r => r.Type)
                .Include(b => b.Billing)
                .Include(b => b.Bookingextraassets)
                    .ThenInclude(bea => bea.ExtraAsset)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Room == null || booking.Room.Type == null)
            {
                booking.Room = await _context.Rooms
                    .Include(r => r.Floor).ThenInclude(f => f.Hotel)
                    .Include(r => r.Type)
                    .FirstOrDefaultAsync(r => r.RoomId == booking.RoomId);
            }

            var room = booking.Room;
            decimal roomBasePrice = room?.Type?.TypePrice ?? 0;

            DateTime start = booking.DateofBooking.ToDateTime(TimeOnly.MinValue);
            DateTime end = booking.EndofBooking.ToDateTime(TimeOnly.MinValue);

            TimeSpan duration = end - start;
            int numberOfNights = (int)Math.Ceiling(duration.TotalDays);

            decimal foodServiceCost = 0;

            decimal totalExtraAssetsPrice = booking.Bookingextraassets.Sum(bea => bea.ExtraAssetPrice);

            decimal roomTotal = roomBasePrice * numberOfNights;
            decimal subTotal = roomTotal + totalExtraAssetsPrice + foodServiceCost;
            decimal taxRate = 0.10m;
            decimal totalTax = subTotal * taxRate;
            decimal totalDue = subTotal + totalTax;

            var viewModel = new BookingInvoiceViewModel
            {
                Booking = booking,
                BillingInfo = booking.Billing,
                Room = booking.Room,
                Hotel = booking.Room?.Floor?.Hotel,

                RoomPrice = roomBasePrice,
                NumberOfNights = numberOfNights,
                TotalExtraAssetsPrice = totalExtraAssetsPrice,
                SubTotal = subTotal,
                TotalDue = totalDue
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,RoomId,BillingId,DateofBooking,EndofBooking,FoodOrderType,Billing")] Booking booking, int[] SelectedExtraAssets)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError(string.Empty, "Oh-Uh: User not identified!");
                return RedirectToAction("Create");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Oh-Uh: User not identified!");
                return RedirectToAction("Create");
            }

            booking.UserId = user.UserId;

            var conflict = await _context.Bookings
                .Where(b => b.RoomId == booking.RoomId)
                .Where(b => booking.DateofBooking < b.EndofBooking && booking.EndofBooking > b.DateofBooking)
                .AnyAsync();

            if (conflict)
            {
                ModelState.AddModelError(string.Empty, "This room is already taken!, Pick another one.");
                return ReturnViewWithViewData(booking);
            }

            if (booking.Billing != null)
            {
                booking.Billing.UserId = user.UserId;
                _context.Billinginfos.Add(booking.Billing);

                try
                {
                    await _context.SaveChangesAsync();
                    booking.BillingId = booking.Billing.BillingId;
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "Error al guardar la información de facturación en la base de datos.");
                    return ReturnViewWithViewData(booking);
                }
            }

            _context.Add(booking);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Error al guardar la reserva en la base de datos. Verifique que todos los campos obligatorios estén llenos.");
                return ReturnViewWithViewData(booking);
            }

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

            return RedirectToAction("Invoice", new { id = booking.BookingId });
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
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,RoomId,BillingId,DateofBooking,EndofBooking,FoodOrderType,Status")] Booking booking)
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