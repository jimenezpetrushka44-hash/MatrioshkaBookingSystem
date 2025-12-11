using MatrioshkaBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


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
}
