
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetRescue.Data;
using PetRescue.Models;

namespace PetRescue.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _he;

        public PetsController(
            ApplicationDbContext db,
            IWebHostEnvironment he)
        {
            _db = db;
            _he = he;
        }

        // GET: Admin/Pets
        public IActionResult Index()
        {
            var pets = _db.Pets
                .Include(p => p.PetType)
                .ToList();

            return View(pets);
        }

        // Load Pet Types for Dropdown
        private void LoadPetTypes(object? selectedPetType = null)
        {
            ViewBag.PetTypeId = new SelectList(
                _db.PetTypes.ToList(),
                "PetTypeId",
                "PetTypeName",
                selectedPetType
            );
        }

        // GET: Admin/Pets/Create
        public IActionResult Create()
        {
            LoadPetTypes();
            return View();
        }

        // POST: Admin/Pets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Pet pet, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                
                if (image != null && image.Length > 0)
                {
                    var imageFolder = Path.Combine(
                        _he.WebRootPath, "Images");

                    Directory.CreateDirectory(imageFolder);

                    var fileName = Guid.NewGuid().ToString()
                        + Path.GetExtension(image.FileName);

                    var filePath = Path.Combine(
                        imageFolder, fileName);

                    using (var stream = new FileStream(
                        filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    pet.ImageUrl = "Images/" + fileName;
                }

                _db.Pets.Add(pet);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            LoadPetTypes(pet.PetTypeId);
            return View(pet);
        }

        // GET: Admin/Pets/Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var pet = _db.Pets.FirstOrDefault(
                p => p.PetId == id);

            if (pet == null)
                return NotFound();

            LoadPetTypes(pet.PetTypeId);
            return View(pet);
        }

        // POST: Admin/Pets/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Pet pet, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                var existingPet = await _db.Pets.FindAsync(
                    pet.PetId);

                if (existingPet == null)
                    return NotFound();

                existingPet.PetTypeId = pet.PetTypeId;
                existingPet.Breed = pet.Breed;
                existingPet.Age = pet.Age;
                existingPet.Gender = pet.Gender;
                existingPet.Description = pet.Description;
                existingPet.Location = pet.Location;
                existingPet.Status = pet.Status;

                // Upload New Image
                if (image != null && image.Length > 0)
                {
                    var imageFolder = Path.Combine(
                        _he.WebRootPath, "Images");

                    Directory.CreateDirectory(imageFolder);

                    var fileName = Guid.NewGuid().ToString()
                        + Path.GetExtension(image.FileName);

                    var filePath = Path.Combine(
                        imageFolder, fileName);

                    using (var stream = new FileStream(
                        filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    existingPet.ImageUrl = "Images/" + fileName;
                }

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            LoadPetTypes(pet.PetTypeId);
            return View(pet);
        }

        // GET: Admin/Pets/Details
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var pet = _db.Pets
                .Include(p => p.PetType)
                .FirstOrDefault(p => p.PetId == id);

            if (pet == null)
                return NotFound();

            return View(pet);
        }

        // GET: Admin/Pets/Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var pet = _db.Pets
                .Include(p => p.PetType)
                .FirstOrDefault(p => p.PetId == id);

            if (pet == null)
                return NotFound();

            return View(pet);
        }

        // POST: Admin/Pets/Delete
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id == null)
                return NotFound();

            var pet = await _db.Pets.FindAsync(id);

            if (pet == null)
                return NotFound();

            _db.Pets.Remove(pet);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
