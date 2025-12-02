using Microsoft.AspNetCore.Mvc;
using MatrioshkaBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrioshkaBookingSystem.Controllers
{
    public class AdminController : Controller
    {
        public readonly BookingDbContext _context;

        public AdminController(BookingDbContext context)
        {
            _context = context;
        }

        public IActionResult Admins()
        {
            ViewData["BodyClass"] = "admin-page";

            var users = _context.Users.ToList();
            var hotels = _context.Hotels.ToList();
            var floors = _context.Floors
                .Include(f => f.Hotel).ToList();
            var rooms = _context.Rooms
                .Include(r => r.Floor)
                .ThenInclude(f => f.Hotel)
                .Include(r => r.Type).ToList();

            var model = new AdminViewModel
            {
                Users = users,
                Hotels = hotels,
                Floors = floors,
                Rooms = rooms

            };

            return View(model);
        }
    }
}
