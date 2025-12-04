using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatrioshkaBookingSystem.Models;

namespace MatrioshkaBookingSystem.Controllers
{
    public class RoomsController : Controller
    {
        private readonly BookingDbContext _context;

        public RoomsController(BookingDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            ViewData["BodyClass"] = "admin-page";

            var rooms = _context.Rooms
                .Include(r => r.Floor)
                .Include(r => r.Type)
                .Include(r => r.Assets);
            return View(await rooms.ToListAsync());
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Rooms/Create
        public IActionResult Create()
        {
            ViewData["BodyClass"] = "admin-page";

            ViewBag.Hotels = _context.Hotels.ToList();
            ViewBag.Floors = _context.Floors
                .Include(f => f.Hotel)
                .ToList();
            ViewBag.RoomTypes = _context.Roomtypes.ToList();
            ViewBag.Assets = _context.Roomassets.ToList();

            return View();
        }

        // POST: Rooms/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("RoomId,FloorId,TypeId,RoomStatus")] Room room,
            int[] SelectedAssetIds) 
        {
            ViewData["BodyClass"] = "admin-page";

            if (SelectedAssetIds != null && SelectedAssetIds.Length > 0)
            {
                room.Assets ??= new List<Roomasset>();

                var selectedAssets = await _context.Roomassets
                    .Where(a => SelectedAssetIds.Contains(a.AssetId))
                    .ToListAsync();

                foreach (var asset in selectedAssets)
                {
                    room.Assets.Add(asset);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction("Admins", "Admin"); 
            }

            ViewBag.Hotels = _context.Hotels.ToList();
            ViewBag.Floors = _context.Floors
                .Include(f => f.Hotel)
                .ToList();
            ViewBag.RoomTypes = _context.Roomtypes.ToList();
            ViewBag.Assets = _context.Roomassets.ToList(); 

            return View(room);
        }
        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["BodyClass"] = "admin-page";

            if (id == null)
                return NotFound();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound();

            ViewBag.Floors = _context.Floors
                .Include(f => f.Hotel)
                .ToList();
            ViewBag.RoomTypes = _context.Roomtypes.ToList();

            return View(room);
        }

        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,FloorId,TypeId,RoomStatus")] Room room)
        {
            if (id != room.RoomId)
                return NotFound();

            ViewData["BodyClass"] = "admin-page";


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.RoomId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Floors = _context.Floors
                .Include(f => f.Hotel)
                .ToList();
            ViewBag.RoomTypes = _context.Roomtypes.ToList();

            return View(room);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            ViewData["BodyClass"] = "admin-page";


            var room = await _context.Rooms
                .Include(r => r.Floor)
                .Include(r => r.Type)
                .FirstOrDefaultAsync(m => m.RoomId == id);

            if (room == null)
                return NotFound();

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
                _context.Rooms.Remove(room);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
        public IActionResult GetAvailableRoomsByHotel (int hotelId)
        {
            var availableRooms = _context.Rooms
                .Include(r => r.Floor)
                .Include(r => r.Type)
                .Where(r => r.Floor.HotelId == hotelId && r.RoomStatus == "Available")
                .Select(r => new
                {
                    roomId = r.RoomId,
                    roomName = $"Room #{r.RoomId} - {r.Type.TypeName} (Floor {r.Floor.FloorNumber})"

                }).ToList();
            return Json(availableRooms);
        }
    }
}
