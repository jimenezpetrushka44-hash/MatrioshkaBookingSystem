using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatrioshkaBookingSystem.Models;

namespace MatrioshkaBookingSystem.Controllers
{

    public class ClientController : Controller
    {
        private readonly BookingDbContext _context;

        public ClientController(BookingDbContext context)
        {
            _context = context;
        }
        public IActionResult Clients()
        {
            ViewData["BodyClass"] = "client-page";
            var hotels = _context.Hotels.ToList();

            var ho = new ClientViewModel
            {
                Hotels = hotels
            };
            return View(ho);
        }
    }
}
