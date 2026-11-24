
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetRescue.Data;
using PetRescue.Models;

namespace PetRescue.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RescueRequestsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _he;

        public RescueRequestsController(
            ApplicationDbContext db,
            IWebHostEnvironment he)
        {
            _db = db;
            _he = he;
        }

        // GET: Admin/RescueRequests
        public IActionResult Index()
        {
            var requests = _db.RescueRequests
                .Include(r => r.PetType)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return View(requests);
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

        // GET: Admin/RescueRequests/Details
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var request = _db.RescueRequests
                .Include(r => r.PetType)
                .FirstOrDefault(r => r.RescueRequestId == id);

            if (request == null)
                return NotFound();

            return View(request);
        }

        // GET: Admin/RescueRequests/Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var request = _db.RescueRequests
                .FirstOrDefault(r => r.RescueRequestId == id);

            if (request == null)
                return NotFound();

            LoadPetTypes(request.PetTypeId);

            return View(request);
        }

        // POST: Admin/RescueRequests/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            RescueRequest request, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                var existingRequest =
                    await _db.RescueRequests.FindAsync(
                        request.RescueRequestId);

                if (existingRequest == null)
                    return NotFound();

                existingRequest.ReporterName =
                    request.ReporterName;

                existingRequest.ContactNumber =
                    request.ContactNumber;

                existingRequest.PetTypeId =
                    request.PetTypeId;

                existingRequest.Description =
                    request.Description;

                existingRequest.Location =
                    request.Location;

                existingRequest.Status =
                    request.Status;

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

                    existingRequest.ImageUrl =
                        "Images/" + fileName;
                }

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            LoadPetTypes(request.PetTypeId);

            return View(request);
        }

        // GET: Admin/RescueRequests/Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var request = _db.RescueRequests
                .Include(r => r.PetType)
                .FirstOrDefault(r => r.RescueRequestId == id);

            if (request == null)
                return NotFound();

            return View(request);
        }

        // POST: Admin/RescueRequests/Delete
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id == null)
                return NotFound();

            var request =
                await _db.RescueRequests.FindAsync(id);

            if (request == null)
                return NotFound();

            _db.RescueRequests.Remove(request);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
