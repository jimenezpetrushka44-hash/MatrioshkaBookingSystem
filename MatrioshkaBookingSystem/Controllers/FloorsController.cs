using MatrioshkaBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class FloorsController : Controller
{
    // Controller
    private readonly BookingDbContext _context;

    // Constructor:
    public FloorsController(BookingDbContext context)
    {
        _context = context;
    }

    // Create get
    public IActionResult Create()
    {
        ViewData["BodyClass"] = "admin-page";
        ViewBag.Hotels = _context.Hotels.ToList();
        return View();
    }

    // Create post

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Floor floor)
    {
        // Validation
        if (ModelState.IsValid)
        {
            _context.Floors.Add(floor);
            _context.SaveChanges();
            return RedirectToAction("Admins", "Admin");
        }

        ViewData["BodyClass"] = "admin-page";
        ViewBag.Hotels = _context.Hotels.ToList();
        return View(floor);
    }

    // Edit GET
    public IActionResult Edit(int id)
    {
        var floor = _context.Floors.FirstOrDefault(f => f.FloorId == id);
        if (floor == null)
            return NotFound();

        ViewBag.Hotels = _context.Hotels.ToList();
        return View(floor);
    }

    // Edit POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Floor updatedFloor)
    {
        var floor = _context.Floors.FirstOrDefault(f => f.FloorId == id);
        if (floor == null)
            return NotFound();

        // Validation:
        if (ModelState.IsValid)
        {
            floor.FloorStatus = updatedFloor.FloorStatus;
            floor.HotelId = updatedFloor.HotelId;

            _context.SaveChanges();
            return RedirectToAction("Admins", "Admin");
        }

        ViewBag.Hotels = _context.Hotels.ToList();
        return View(updatedFloor);
    }

    // Delete egt
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var floor = await _context.Floors
            .Include(f => f.Rooms)
            .FirstOrDefaultAsync(f => f.FloorId == id);

        if (floor == null)
            return NotFound();

        return View(floor);
    }

    // Delete post

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var floor = await _context.Floors
            .Include(f => f.Rooms)
            .FirstOrDefaultAsync(f => f.FloorId == id);

        // Validation
        if (floor != null)
        {
            if (floor.Rooms.Any())
                return BadRequest();

            _context.Floors.Remove(floor);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Admins", "Admin");
    }
}
