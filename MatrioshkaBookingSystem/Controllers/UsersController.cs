using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MatrioshkaBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace MatrioshkaBookingSystem.Controllers
{
    // Only admins :
    [Authorize(Roles = "Admin")]

    // Controller:
    public class UsersController : Controller
    {
        private readonly BookingDbContext _context;

        // Contructor
        public UsersController(BookingDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        // Create post:
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string firstName,
            string lastName,
            string email,
            string phone,
            string username,
            string password)
        {
            // Validation:
            if (_context.Users.Any(u => u.Username == username))
            {
                ModelState.AddModelError("", "Username already exists.");
                return View();
            }

            // new user
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Username = username,
                UserPassword = password
            };

            // Adding the new user
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //roles:
            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Client");

            //validating roles:
            if (defaultRole != null)
            {
                user.Roles.Add(defaultRole);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        // Index get
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // detaild get
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // edit get:
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return NotFound();

            var roles = await _context.Roles.ToListAsync();
            int? currentRoleId = user.Roles.FirstOrDefault()?.RoleId;

            ViewData["RoleId"] = new SelectList(roles, "RoleId", "RoleName", currentRoleId);

            return View(user);
        }

        // edit post

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int RoleId)
        {
            var userToUpdate = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (userToUpdate == null)
                return NotFound();

            if (await TryUpdateModelAsync<User>(
                userToUpdate,
                "",
                u => u.FirstName, u => u.LastName, u => u.Username, u => u.Email, u => u.Phone))
            {
                var newRole = await _context.Roles.FindAsync(RoleId);

                if (newRole != null)
                {
                    userToUpdate.Roles.Clear();
                    userToUpdate.Roles.Add(newRole);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Admins", "Admin");
            }

            var roles = await _context.Roles.ToListAsync();
            ViewData["RoleId"] = new SelectList(roles, "RoleId", "RoleName", RoleId);

            return View(userToUpdate);
        }

        // delete get
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // delete post
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user != null)
            {
                user.Roles.Clear();
                await _context.SaveChangesAsync();

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Admins", "Admin");
        }

    }
}
