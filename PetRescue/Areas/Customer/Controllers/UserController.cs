
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PetRescue.Models;

namespace PetRescue.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: User/Index
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ApplicationUser user, string password)
        {
            if (ModelState.IsValid)
            {
                user.UserName = user.Email;

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    TempData["Save"] = "User has been registered successfully!";
                    return RedirectToAction("Login", "Account",
                        new { area = "Identity" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(user);
        }

        // GET: User/Edit
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: User/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            var userInfo = await _userManager.FindByIdAsync(user.Id);

            if (userInfo == null)
                return NotFound();

            userInfo.FirstName = user.FirstName;
            userInfo.LastName = user.LastName;

            var result = await _userManager.UpdateAsync(userInfo);

            if (result.Succeeded)
            {
                TempData["Save"] = "User has been updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }

        // GET: User/Details
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            return View(user);
        }
    }
}
//