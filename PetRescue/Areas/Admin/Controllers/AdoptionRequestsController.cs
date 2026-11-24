
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetRescue.Data;
using PetRescue.Models;

namespace PetRescue.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdoptionRequestsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdoptionRequestsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Admin/AdoptionRequests
        public IActionResult Index()
        {
            var requests = _db.AdoptionRequests
                .Include(r => r.Pet)
                .OrderByDescending(r => r.RequestDate)
                .ToList();

            return View(requests);
        }

        // GET: Admin/AdoptionRequests/Details
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var request = _db.AdoptionRequests
                .Include(r => r.Pet)
                .Include(r => r.User)
                .FirstOrDefault(r => r.AdoptionRequestId == id);

            if (request == null)
                return NotFound();

            return View(request);
        }

        // GET: Admin/AdoptionRequests/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var request = await _db.AdoptionRequests
                .Include(r => r.Pet)
                .FirstOrDefaultAsync(r => r.AdoptionRequestId == id);

            if (request == null)
                return NotFound();

            return View(request);
        }
        // POST: Admin/AdoptionRequests/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int AdoptionRequestId, string Status)
        {
            var request = await _db.AdoptionRequests
                .Include(r => r.Pet)
                .FirstOrDefaultAsync(r => r.AdoptionRequestId == AdoptionRequestId);

            if (request == null)
                return NotFound();

            if (Status != "Pending" &&
                Status != "Approved" &&
                Status != "Rejected")
            {
                TempData["error"] = "Invalid status.";
                return RedirectToAction(nameof(Index));
            }

            if (request.Pet == null)
            {
                TempData["error"] = "Pet not found.";
                return RedirectToAction(nameof(Index));
            }

            string oldStatus = request.Status;

            
            request.Status = Status;

           
            if (Status == "Approved")
            {
                request.Pet.Status = "Adopted";

               
                var otherRequests = await _db.AdoptionRequests
                    .Where(a => a.PetId == request.PetId
                                && a.AdoptionRequestId != request.AdoptionRequestId
                                && a.Status == "Pending")
                    .ToListAsync();

                foreach (var other in otherRequests)
                {
                    other.Status = "Rejected";
                }
            }

            
            else if (oldStatus == "Approved")
            {
                bool anotherApprovedRequest = await _db.AdoptionRequests
                    .AnyAsync(a => a.PetId == request.PetId
                                   && a.AdoptionRequestId != request.AdoptionRequestId
                                   && a.Status == "Approved");

                if (!anotherApprovedRequest)
                {
                    request.Pet.Status = "Available";
                }
            }

            await _db.SaveChangesAsync();

            TempData["save"] = "Adoption request status updated successfully!";

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin AdoptionRequests Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var request = _db.AdoptionRequests
                .Include(r => r.Pet)
                .Include(r => r.User)
                .FirstOrDefault(r => r.AdoptionRequestId == id);

            if (request == null)
                return NotFound();

            return View(request);
        }

        // POST: Admin AdoptionRequests Delete
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id == null)
                return NotFound();

            var request = await _db.AdoptionRequests
                .FindAsync(id);

            if (request == null)
                return NotFound();

            _db.AdoptionRequests.Remove(request);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // GET: Admin AdoptionRequests Approve
        public async Task<IActionResult> Approve(int? id)
        {
            if (id == null)
                return NotFound();

            var request = await _db.AdoptionRequests
                .Include(a => a.Pet)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AdoptionRequestId == id);

            if (request == null)
                return NotFound();

            if (request.Status != "Pending")
            {
                TempData["error"] = "This request has already been processed.";
                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }
        //post: Approve Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _db.AdoptionRequests
                .Include(a => a.Pet)
                .FirstOrDefaultAsync(a => a.AdoptionRequestId == id);

            if (request == null)
            {
                TempData["error"] = "Adoption request not found.";
                return RedirectToAction(nameof(Index));
            }

            if (request.Status != "Pending")
            {
                TempData["error"] = "This request has already been processed.";
                return RedirectToAction(nameof(Index));
            }

            if (request.Pet == null)
            {
                TempData["error"] = "Pet not found.";
                return RedirectToAction(nameof(Index));
            }

            
            request.Status = "Approved";

           
            request.Pet.Status = "Adopted";

            
            var otherRequests = await _db.AdoptionRequests
                .Where(a => a.PetId == request.PetId
                            && a.AdoptionRequestId != id
                            && a.Status == "Pending")
                .ToListAsync();

            foreach (var other in otherRequests)
            {
                other.Status = "Rejected";
            }

            await _db.SaveChangesAsync();

            TempData["save"] = "Adoption request approved successfully!";

            return RedirectToAction(nameof(Index));
        }
        //Get: AdopationReuest Admin Reject
        
        public async Task<IActionResult> Reject(int? id)
        {
            if (id == null)
                return NotFound();

            var request = await _db.AdoptionRequests
                .Include(a => a.Pet)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AdoptionRequestId == id);

            if (request == null)
                return NotFound();

            if (request.Status != "Pending")
            {
                TempData["error"] = "This request has already been processed.";
                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }
        // post : Reject Action Model
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var request = await _db.AdoptionRequests
                .FirstOrDefaultAsync(a => a.AdoptionRequestId == id);

            if (request == null)
                return NotFound();

            if (request.Status != "Pending")
            {
                TempData["error"] = "This request has already been processed.";
                return RedirectToAction(nameof(Index));
            }

            request.Status = "Rejected";

            await _db.SaveChangesAsync();

            TempData["save"] = "Adoption request rejected successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
