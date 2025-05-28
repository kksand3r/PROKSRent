using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PROKSRent.Data;
using PROKSRent.Models;

namespace PROKSRent.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars.Include(c => c.FuelType).Include(c => c.TransmissionType).Include(c => c.Brand).Include(c => c.Bookings).ToListAsync();
            return View(cars);
        }

        public IActionResult Create()
        {
            ViewBag.FuelTypes = new SelectList(_context.FuelTypes, "Id", "Name");
            ViewBag.TransmissionTypes = new SelectList(_context.TransmissionTypes, "Id", "Name");
            ViewBag.Brands = new SelectList(_context.Brands, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car)
        {
            if (ModelState.IsValid)
            {
                if (car.CarImage is not null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string uploadsFolder = Path.Combine(wwwRootPath, "img", "cars");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string fileExt = Path.GetExtension(car.CarImage.FileName);
                    string fileName = $"{Guid.NewGuid()}{fileExt}";
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    string dbPath = $"/img/cars/{fileName}";

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await car.CarImage.CopyToAsync(fileStream);
                    }

                    car.ImagePath = dbPath;
                }

                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            ViewBag.FuelTypes = new SelectList(_context.FuelTypes, "Id", "Name");
            ViewBag.TransmissionTypes = new SelectList(_context.TransmissionTypes, "Id", "Name");
            ViewBag.Brands = new SelectList(_context.Brands, "Id", "Name");
            return View(car);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            ViewBag.FuelTypes = new SelectList(_context.FuelTypes, "Id", "Name");
            ViewBag.TransmissionTypes = new SelectList(_context.TransmissionTypes, "Id", "Name");
            ViewBag.Brands = new SelectList(_context.Brands, "Id", "Name");
            return View(car);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Car car)
        {
            if (ModelState.IsValid)
            {
                var existingCar = await _context.Cars.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                if (existingCar == null)
                {
                    return NotFound();
                }

                if (car.CarImage is not null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string uploadsFolder = Path.Combine(wwwRootPath, "img", "cars");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string fileExt = Path.GetExtension(car.CarImage.FileName);
                    string fileName = $"{Guid.NewGuid()}{fileExt}";
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    string dbPath = $"/img/cars/{fileName}";

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await car.CarImage.CopyToAsync(fileStream);
                    }
                    car.ImagePath = dbPath;
                }
                else
                {
                    car.ImagePath = existingCar.ImagePath;
                }
                _context.Update(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.FuelTypes = new SelectList(_context.FuelTypes, "Id", "Name");
            ViewBag.TransmissionTypes = new SelectList(_context.TransmissionTypes, "Id", "Name");
            ViewBag.Brands = new SelectList(_context.Brands, "Id", "Name");
            return View(car);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var car = await _context.Cars.Include(c => c.FuelType).Include(c => c.TransmissionType).Include(c => c.Brand).FirstOrDefaultAsync(m => m.Id == id);
            return View(car);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Map()
        {
            return View();

        }
        [HttpGet]

        public JsonResult GetCarLocations()
        {
            var random = new Random();
            var lutskCenter = new { Lat = 50.7479, Lng = 25.3293 };
            var range = 0.1;

            var cars = _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.Bookings)
                .Where(c => c.Bookings.Any(b => b.IsConfirmed))
                .ToList()
                .Select((c, index) => new
                {
                    c.Id,
                    Brand = c.Brand?.Name ?? "",
                    c.Model,
                    c.CarNumber,
                    Latitude = lutskCenter.Lat + (random.NextDouble() - 0.5) * range * (1 + index * 0.01),
                    Longitude = lutskCenter.Lng + (random.NextDouble() - 0.5) * range * (1 + index * 0.01)
                })
                .ToList();
            return Json(cars);
        }
    }
}
