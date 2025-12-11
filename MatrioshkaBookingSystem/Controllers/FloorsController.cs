using MatrioshkaBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class FloorsController : Controller
{
    private readonly BookingDbContext _context;

    public FloorsController(BookingDbContext context)
    {
        _context = context;
    }

    public IActionResult Create()
    {
        ViewData["BodyClass"] = "admin-page";
        ViewBag.Hotels = _context.Hotels.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Floor floor)
    {
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

    public IActionResult Edit(int id)
    {
        var floor = _context.Floors.FirstOrDefault(f => f.FloorId == id);
        if (floor == null)
            return NotFound();

        ViewBag.Hotels = _context.Hotels.ToList();
        return View(floor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Floor updatedFloor)
    {
        var floor = _context.Floors.FirstOrDefault(f => f.FloorId == id);
        if (floor == null)
            return NotFound();

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

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var floor = await _context.Floors
            .Include(f => f.Rooms)
            .FirstOrDefaultAsync(f => f.FloorId == id);

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
