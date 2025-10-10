using Microsoft.AspNetCore.Mvc;
using SelfCheckoutSystem.Data;

namespace SelfCheckoutSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StartShopping()
        {
            return RedirectToAction("Scan", "Checkout");
        }
    }
}