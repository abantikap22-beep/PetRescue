
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetRescue.Data;
using PetRescue.Models;

namespace PetRescue.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AdoptionController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdoptionController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: Adoption Checkout
       
        public async Task<IActionResult> Checkout(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Cart", "Home",
                    new { area = "Customer" });
            }

            var pet = await _db.Pets
                .FirstOrDefaultAsync(p => p.PetId == id);

            if (pet == null || pet.Status != "Available")
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var request = new AdoptionRequest
            {
                PetId = pet.PetId,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email ?? "",
                Phone = user.PhoneNumber ?? ""
            };

            ViewBag.Pet = pet;

            return View(request);
        }

        // POST: Adoption Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(AdoptionRequest request)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var pet = await _db.Pets
                .FirstOrDefaultAsync(p => p.PetId == request.PetId);

            if (pet == null)
            {
                return NotFound();
            }

            if (pet.Status != "Available")
            {
                ModelState.AddModelError(
                    "",
                    "This pet is no longer available."
                );
            }

            request.UserId = user.Id;
            request.Status = "Pending";
            request.RequestDate = DateTime.Now;

            ModelState.Remove(nameof(request.UserId));
            ModelState.Remove(nameof(request.Status));
            ModelState.Remove(nameof(request.RequestDate));

            if (ModelState.IsValid)
            {
                _db.AdoptionRequests.Add(request);

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(OrderConfirmation));
            }

            ViewBag.Pet = pet;

            return View(request);
        }

        // GET: Order Confirmation
        [HttpGet]
        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}
