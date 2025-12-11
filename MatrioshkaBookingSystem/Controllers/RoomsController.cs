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

        public async Task<IActionResult> Index()
        {
            ViewData["BodyClass"] = "admin-page";

            var rooms = _context.Rooms
                .Include(r => r.Floor)
                .Include(r => r.Type)
                .Include(r => r.Assets);

            return View(await rooms.ToListAsync());
        }

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

        public IActionResult Create()
        {
            ViewData["BodyClass"] = "admin-page";

            ViewBag.Floors = new SelectList(
                _context.Floors.Include(f => f.Hotel).ToList(),
                "FloorId",
                "FloorNumber"
            );

            ViewBag.RoomTypes = new SelectList(
                _context.Roomtypes.ToList(),
                "TypeId",
                "TypeName"
            );

            ViewBag.Assets = _context.Roomassets.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,FloorId,TypeId,RoomStatus")] Room room, int[] SelectedAssetIds)
        {
            ViewData["BodyClass"] = "admin-page";

            if (SelectedAssetIds != null && SelectedAssetIds.Length > 0)
            {
                room.Assets = new List<Roomasset>();
                var selectedAssets = await _context.Roomassets
                    .Where(a => SelectedAssetIds.Contains(a.AssetId))
                    .ToListAsync();

                foreach (var asset in selectedAssets)
                    room.Assets.Add(asset);
            }

            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction("Admins", "Admin");
            }

            ViewBag.Floors = new SelectList(
                _context.Floors.Include(f => f.Hotel).ToList(),
                "FloorId",
                "FloorNumber",
                room.FloorId
            );

            ViewBag.RoomTypes = new SelectList(
                _context.Roomtypes.ToList(),
                "TypeId",
                "TypeName",
                room.TypeId
            );

            ViewBag.Assets = _context.Roomassets.ToList();

            return View(room);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var room = await _context.Rooms
                .Include(r => r.Assets)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                return NotFound();

            ViewData["BodyClass"] = "admin-page";

            ViewBag.Floors = new SelectList(
                _context.Floors.Include(f => f.Hotel).ToList(),
                "FloorId",
                "FloorNumber",
                room.FloorId
            );

            ViewBag.RoomTypes = new SelectList(
                _context.Roomtypes.ToList(),
                "TypeId",
                "TypeName",
                room.TypeId
            );

            ViewBag.Assets = _context.Roomassets.ToList();

            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,FloorId,TypeId,RoomStatus")] Room updatedRoom, int[] SelectedAssetIds)
        {
            if (id != updatedRoom.RoomId)
                return NotFound();

            var room = await _context.Rooms
                .Include(r => r.Assets)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                return NotFound();

            room.FloorId = updatedRoom.FloorId;
            room.TypeId = updatedRoom.TypeId;
            room.RoomStatus = updatedRoom.RoomStatus;

            room.Assets.Clear();

            if (SelectedAssetIds != null && SelectedAssetIds.Length > 0)
            {
                var selectedAssets = await _context.Roomassets
                    .Where(a => SelectedAssetIds.Contains(a.AssetId))
                    .ToListAsync();

                foreach (var asset in selectedAssets)
                    room.Assets.Add(asset);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Admins", "Admin");
        }

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

            ViewData["BodyClass"] = "admin-page";

            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room != null)
                _context.Rooms.Remove(room);

            await _context.SaveChangesAsync();
            return RedirectToAction("Admins", "Admin");
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomId == id);
        }

        public IActionResult GetFloorsByHotel(int hotelId)
        {
            var floors = _context.Floors
                .Where(f => f.HotelId == hotelId)
                .Select(f => new
                {
                    floorId = f.FloorId,
                    floorStatus = f.FloorStatus
                })
                .ToList();

            return Json(floors);
        }

        public IActionResult GetAvailableRoomsByHotel(int hotelId)
        {
            var availableRooms = _context.Rooms
                .Include(r => r.Floor)
                .Include(r => r.Type)
                .Where(r => r.Floor.HotelId == hotelId && r.RoomStatus == "Available")
                .Select(r => new
                {
                    roomId = r.RoomId,
                    roomName = $"Room #{r.RoomId} - {r.Type.TypeName} (Floor {r.Floor.FloorNumber})"
                })
                .ToList();

            return Json(availableRooms);
        }
    }
}
