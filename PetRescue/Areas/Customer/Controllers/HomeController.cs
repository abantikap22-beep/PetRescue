
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetRescue.Models;
using PetRescue.Data;
using PetRescue.utility;
using PetRescue.Areas.Admin.Controllers;

namespace PetRescue.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ApplicationDbContext db,
            ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index(int? page)
        {
            var pets = _db.Pets
                .Include(p => p.PetType)
                .Where(p => p.Status == "Available")
                .ToList();

            return View(pets);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
        public IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = _db.Pets
                .Include(p => p.PetType)
                .FirstOrDefault(p => p.PetId == id);

            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }
        //Post: Pets detail action Method
        [HttpPost]
        [ActionName("Detail")]
        public IActionResult PetDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = _db.Pets
                .Include(p => p.PetType)
                .FirstOrDefault(p => p.PetId == id);

            if (pet == null)
            {
                return NotFound();
            }

            List<int> pets = HttpContext.Session.Get<List<int>>("pets");

            if (pets == null)
            {
                pets = new List<int>();
            }

            if (!pets.Contains(pet.PetId))
            {
                pets.Add(pet.PetId);
            }

            HttpContext.Session.Set("pets", pets);

            return View(pet);
        }
        public IActionResult Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<int> pets = HttpContext.Session.Get<List<int>>("pets")
                             ?? new List<int>();

            if (pets.Contains(id.Value))
            {
                pets.Remove(id.Value);
            }

            HttpContext.Session.Set("pets", pets);

            return RedirectToAction(nameof(Detail), new { id = id });
        }
        // Get: Pet Cart Action Method
        public IActionResult Cart()
        {
            List<int> petIds = HttpContext.Session.Get<List<int>>("pets")
                               ?? new List<int>();

            List<Pet> pets = _db.Pets
                            .Include(p => p.PetType)
                            .Where(p => petIds.Contains(p.PetId))
                            .ToList();

            return View(pets);
        }
       
        
    }
}
