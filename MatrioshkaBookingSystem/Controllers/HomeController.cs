using System.Diagnostics;
using MatrioshkaBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MatrioshkaBookingSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MatrioshkaBookingSystem.Controllers
{
    // Controller
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookingDbContext _context;

        // Constructor
        public HomeController(ILogger<HomeController> logger, BookingDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        //Index get
        public IActionResult Index()
        {
            return View();
        }

        // Privacy get:
        public IActionResult Privacy()
        {
            return View();
        }

        // Login Post
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Whole Login validation:
            var user = _context.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Username == username && u.UserPassword == password);

            // In case is null 
            if (user == null)
            {
                ViewBag.Error = "Wrong username or password!";
                return View("Index");
            }

            var userRoleName = user.Roles.FirstOrDefault()?.RoleName;

            // in case there's no role in user
            if (userRoleName == null)
            {
                ViewBag.Error = "User has no roles assigned!";
                return View("Index");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, userRoleName)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirecting based on the role: 
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity));

            if (userRoleName == "Admin")
                return RedirectToAction("Admins", "Admin");

            if (userRoleName == "Client")
                return RedirectToAction("Clients", "Client");

            ViewBag.Error = "Invalid role.";
            return View("Index");
        }

        // Logout get
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme); 

            return RedirectToAction("Index", "Home");
        }

        // Admins get:
        public IActionResult Admins()
        {
            var vm = new AdminViewModel
            {
                Users = _context.Users.ToList(),
                Hotels = _context.Hotels.ToList(),
                Floors = _context.Floors
                .Include(f => f.Hotel)
                .ToList(),
                Rooms = _context.Rooms
                .Include(r => r.Floor)
                .Include(r => r.Type)
                .ToList()
            };

            return View(vm);
        }

        // Clients get
        public IActionResult Clients()
        {
            var hotels = _context.Hotels.ToList();

            return View(hotels);
        }

        // Register get
        public IActionResult Register()
        {
            return View();
        }

        // Register post
        [HttpPost]
        public IActionResult Register(string firstName, string lastName, string email, string phone,
                                     string username, string password, string role)
        {
            // Creating user 
            User newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Username = username,
                UserPassword = password,
                Roles = new List<Role>()
            };
            // Adding user

            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Assigning role
            var selectedRole = _context.Roles.FirstOrDefault(r => r.RoleName == role);

            // Validation:
            if (selectedRole != null)
            {
                newUser.Roles.Add(selectedRole);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}