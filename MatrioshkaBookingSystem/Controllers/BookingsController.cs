using MatrioshkaBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
            var bookings = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room).ThenInclude(r => r.Type)
                .Include(b => b.Room).ThenInclude(r => r.Floor).ThenInclude(f => f.Hotel)
                .Include(b => b.Billing);

            return View(await bookings.ToListAsync());
        }

        public IActionResult Create(int hotelId)
        {
            var hotel = _context.Hotels.Find(hotelId);

            if (hotel == null)
                return NotFound();

            ViewBag.SelectedHotelName = hotel.HotelName;
            ViewBag.SelectedHotelId = hotelId;

            var availableRooms = _context.Rooms
                .Include(r => r.Type)
                .Include(r => r.Floor)
                .Where(r => r.Floor.HotelId == hotelId && r.RoomStatus == "Available")
                .Select(r => new
                {
                    RoomId = r.RoomId,
                    RoomDisplay = $"Room {r.RoomId} - {r.Type.TypeName} (${r.Type.TypePrice})"
                })
                .ToList();

            ViewData["RoomId"] = new SelectList(availableRooms, "RoomId", "RoomDisplay");

            ViewBag.ExtraAssets = _context.Extraassets
                .Where(e => e.ExtraAssetStatus == "Available")
                .ToList();

            return View(new Booking());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("RoomId,DateofBooking,EndofBooking")] Booking booking,
            Billinginfo Billing,
            int[] SelectedExtraAssets)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                return RedirectToAction("Login", "Account");

            booking.UserId = user.UserId;

            var conflict = _context.Bookings
                .Where(b => b.RoomId == booking.RoomId)
                .Where(b => booking.DateofBooking < b.EndofBooking &&
                            booking.EndofBooking > b.DateofBooking)
                .Any();

            if (conflict)
            {
                ModelState.AddModelError("", "This room is already booked for those dates.");
                return Redirect(Request.Headers["Referer"].ToString());
            }

            if (Billing != null)
            {
                Billing.UserId = user.UserId;
                _context.Billinginfos.Add(Billing);
                await _context.SaveChangesAsync();
                booking.BillingId = Billing.BillingId;
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            if (SelectedExtraAssets != null && SelectedExtraAssets.Length > 0)
            {
                foreach (var id in SelectedExtraAssets)
                {
                    var extra = _context.Extraassets.Find(id);
                    if (extra != null)
                    {
                        _context.Bookingextraassets.Add(new Bookingextraasset
                        {
                            BookingId = booking.BookingId,
                            ExtraAssetId = extra.ExtraAssetId,
                            ExtraAssetPrice = extra.AssetPrice
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Invoice", new { id = booking.BookingId });
        }

        public async Task<IActionResult> Invoice(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Billing)
                .Include(b => b.Room).ThenInclude(r => r.Type)
                .Include(b => b.Room).ThenInclude(r => r.Floor).ThenInclude(f => f.Hotel)
                .Include(b => b.Bookingextraassets).ThenInclude(be => be.ExtraAsset)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            var roomPrice = booking.Room.Type.TypePrice;
            var nights = (int)(booking.EndofBooking.ToDateTime(TimeOnly.MinValue)
                             - booking.DateofBooking.ToDateTime(TimeOnly.MinValue)).TotalDays;

            var extras = booking.Bookingextraassets.Sum(e => e.ExtraAssetPrice);
            var subtotal = (roomPrice * nights) + extras;
            var tax = subtotal * 0.10m;
            var total = subtotal + tax;

            var vm = new BookingInvoiceViewModel
            {
                Booking = booking,
                BillingInfo = booking.Billing,
                Room = booking.Room,
                Hotel = booking.Room.Floor.Hotel,
                NumberOfNights = nights,
                RoomPrice = roomPrice,
                TotalExtraAssetsPrice = extras,
                SubTotal = subtotal,
                TotalDue = total
            };

            return View(vm);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .Include(b => b.Billing)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Admins", "Admin");
        }
    }
}
