using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROKSRent.Data;
using PROKSRent.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using PROKSRent.Services;

namespace PROKSRent.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public BookingController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars.Include(c => c.FuelType).Include(c => c.TransmissionType).ToListAsync();
            return View(cars);
        }

        [Authorize]
        public async Task<IActionResult> Create(int carId)
        {
            var car = await _context.Cars.FindAsync(carId);

            var booking = new Booking
            {
                CarId = carId,
                Car = car,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                UserId = _userManager.GetUserId(User),
                IsConfirmed = false
            };
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.UserId = _userManager.GetUserId(User);
                booking.IsConfirmed = false;

                _context.Add(booking);
                await _context.SaveChangesAsync();

                var user = await _userManager.GetUserAsync(User);
                var email = user?.Email;
                if (!string.IsNullOrEmpty(email))
                {
                    var confirmUrl = Url.Action("Confirm", "Booking", new { id = booking.Id }, Request.Scheme);

                    var car = await _context.Cars.Include(c => c.Brand).FirstOrDefaultAsync(c => c.Id == booking.CarId);
                    var brand = car?.Brand?.Name ?? "Невідомий бренд";
                    var model = car?.Model ?? "Невідома модель";

                    var message = $@"
                Доброго дня!<br/>
                Ви зробили бронювання автомобіля:<br/>
                <b>{brand} {model}</b><br/>
                З {booking.StartDate:dd.MM.yyyy} по {booking.EndDate:dd.MM.yyyy}.<br/>
                Будь ласка, підтвердіть ваше бронювання за посиланням:<br/>
                <a href='{confirmUrl}'>Підтвердити бронювання</a>
            ";

                    await _emailSender.SendEmailAsync(email, "Підтвердження бронювання автомобіля", message);
                }

                TempData["SuccessMessage"] = "Бронювання створено! Для підтвердження перевірте вашу пошту.";
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }
        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            if (booking.UserId != _userManager.GetUserId(User))
                return Forbid();

            booking.IsConfirmed = true;

            var car = await _context.Cars.FindAsync(booking.CarId);
            if (car != null)
                car.IsAvailable = false;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ваше бронювання підтверджено!";
            return RedirectToAction(nameof(Index));
        }
    }
}
