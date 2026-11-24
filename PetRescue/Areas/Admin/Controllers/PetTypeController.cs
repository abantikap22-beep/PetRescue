
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetRescue.Data;
using PetRescue.Models;

namespace PetRescue.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PetTypeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PetTypeController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Admin/PetType
        public IActionResult Index()
        {
            var petTypes = _db.PetTypes.ToList();
            return View(petTypes);
        }
        // GET: Admin/PetType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/PetType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PetType petType)
        {
            if (ModelState.IsValid)
            {
                _db.PetTypes.Add(petType);
                await _db.SaveChangesAsync();

                TempData["save"] = "Pet Type has been saved successfully";

                return RedirectToAction(nameof(Index));
            }

            return View(petType);
        }

        // GET Admin/PetType/Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var petType = _db.PetTypes.Find(id);

            if (petType == null)
            {
                return NotFound();
            }

            return View(petType);
        }

        // POST Admin/PetType/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PetType petType)
        {
            if (ModelState.IsValid)
            {
                _db.PetTypes.Update(petType);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(petType);
        }

        // GET Admin/PetType/Details
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var petType = _db.PetTypes.Find(id);

            if (petType == null)
            {
                return NotFound();
            }

            return View(petType);
        }

        // GET Admin/PetType/Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var petType = _db.PetTypes.Find(id);

            if (petType == null)
            {
                return NotFound();
            }

            return View(petType);
        }

        // POST Admin/PetType/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var petType = await _db.PetTypes.FindAsync(id);

            if (petType == null)
            {
                return NotFound();
            }

            _db.PetTypes.Remove(petType);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
