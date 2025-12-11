using MatrioshkaBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MatrioshkaBookingSystem.Controllers
{
    public class RoomsController : Controller
    {
        private readonly BookingDbContext _context;

        public RoomsController(BookingDbContext context)
        {
            _context = context;
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            ViewData["BodyClass"] = "admin-page";

            var rooms = _context.Rooms
                .Include(r => r.Floor)
                .Include(r => r.Type)
                .Include(r => r.Assets);

            return View(await rooms.ToListAsync());
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var room = await _context.Rooms
                .Include(r => r.Floor)
                .Include(r => r.Type)
                .Include(r => r.Assets)
                .FirstOrDefaultAsync(m => m.RoomId == id);

            if (room == null)
                return NotFound();

            return View(room);
        }

        // CREATE GET
        public IActionResult Create()
        {
            ViewData["BodyClass"] = "admin-page";

            ViewBag.Hotels = _context.Hotels.ToList();
            ViewBag.RoomTypes = _context.Roomtypes.ToList();
            ViewBag.Assets = _context.Roomassets.ToList();

            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int FloorId, int TypeId, string RoomStatus, int[] SelectedAssetIds)
        {
            // VALIDATION: Floor must exist
            var floor = await _context.Floors.FindAsync(FloorId);
            if (floor == null)
            {
                TempData["Error"] = "The selected floor does not exist.";
                return RedirectToAction("Create");
            }

            // CREATE ROOM
            var room = new Room
            {
                FloorId = FloorId,
                TypeId = TypeId,
                RoomStatus = RoomStatus,
                Assets = new List<Roomasset>()
            };

            // ASSETS
            if (SelectedAssetIds != null)
            {
                var selectedAssets = await _context.Roomassets
                    .Where(a => SelectedAssetIds.Contains(a.AssetId))
                    .ToListAsync();

                foreach (var asset in selectedAssets)
                    room.Assets.Add(asset);
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return RedirectToAction("Admins", "Admin");
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var room = await _context.Rooms
                .Include(r => r.Floor)
                .Include(r => r.Assets)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                return NotFound();

            ViewData["BodyClass"] = "admin-page";

            ViewBag.Hotels = _context.Hotels.ToList();

            ViewBag.Floors = _context.Floors
                .Where(f => f.HotelId == room.Floor.HotelId)
                .ToList();

           
            ViewBag.RoomTypes = _context.Roomtypes.ToList();

            ViewBag.Assets = _context.Roomassets.ToList();

            return View(room);
        }


        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int FloorId, int TypeId, string RoomStatus, int[] SelectedAssetIds)
        {
            var room = await _context.Rooms
                .Include(r => r.Assets)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                return NotFound();

            room.FloorId = FloorId;
            room.TypeId = TypeId;
            room.RoomStatus = RoomStatus;

            room.Assets.Clear();

            if (SelectedAssetIds != null)
            {
                var newAssets = await _context.Roomassets
                    .Where(a => SelectedAssetIds.Contains(a.AssetId))
                    .ToListAsync();

                foreach (var asset in newAssets)
                    room.Assets.Add(asset);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Admins", "Admin");
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var room = await _context.Rooms
                .Include(r => r.Floor)
                .Include(r => r.Type)
                .FirstOrDefaultAsync(m => m.RoomId == id);

            if (room == null)
                return NotFound();

            return View(room);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Assets)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room != null)
            {
                room.Assets.Clear();
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Admins", "Admin");
        }

        // AJAX: FLOORS BY HOTEL
        public IActionResult GetFloorsByHotel(int hotelId)
        {
            var floors = _context.Floors
                .Where(f => f.HotelId == hotelId)
                .Select(f => new
                {
                    floorId = f.FloorId,
                    floorNumber = f.FloorNumber,
                    floorStatus = f.FloorStatus
                })
                .ToList();

            return Json(floors);
        }
    }
}
