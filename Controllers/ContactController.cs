using Microsoft.AspNetCore.Mvc;

namespace PROKSRent.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendMessage(string name, string email, string phone, string subject, string message)
        {
            ViewBag.Message = "Дякуємо! Ваше повідомлення відправлено.";
            return View("Index");
        }
    }
}
