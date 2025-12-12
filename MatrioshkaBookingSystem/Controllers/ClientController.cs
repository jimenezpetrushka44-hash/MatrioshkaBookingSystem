using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatrioshkaBookingSystem.Models;

namespace MatrioshkaBookingSystem.Controllers
{
    // Controller
    public class ClientController : Controller
    {
        private readonly BookingDbContext _context;

        // Contructor:
        public ClientController(BookingDbContext context)
        {
            _context = context;
        }

        // Clients:
        public IActionResult Clients()
        {
            ViewData["BodyClass"] = "client-page";
            var hotels = _context.Hotels.ToList();

            // creating view model
            var ho = new ClientViewModel
            {
                Hotels = hotels
            };
            return View(ho);
        }
    }
}
