using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfCheckoutSystem.Data;
using SelfCheckoutSystem.Models;

namespace SelfCheckoutSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dashboard
        public IActionResult Dashboard()
        {
            var todaySales = _context.Orders
                .Where(o => o.OrderDate.Date == DateTime.Today)
                .Sum(o => o.TotalAmount);

            var todayOrders = _context.Orders
                .Count(o => o.OrderDate.Date == DateTime.Today);

            ViewBag.TodaySales = todaySales;
            ViewBag.TodayOrders = todayOrders;

            return View();
        }

        // Orders
        public IActionResult Orders()
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // Order Details
        public IActionResult OrderDetails(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // Products
        public IActionResult Products()
        {
            var products = _context.Products
                .Include(p => p.Category)
                .ToList();

            return View(products);
        }

        // Create Product
        public IActionResult CreateProduct()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Products));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        // Edit Product
        public IActionResult EditProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Products));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        // Delete Product
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.IsActive = false;
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Products));
        }

        // Categories
        public IActionResult Categories()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        // Reports
        public IActionResult Reports(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.Today.AddDays(-30);
            endDate ??= DateTime.Today;

            var orders = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToList();

            var totalSales = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;

            var topProducts = orders
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => oi.Product.Name)
                .Select(g => new { ProductName = g.Key, Quantity = g.Sum(oi => oi.Quantity) })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToList();

            var paymentMethods = orders
                .GroupBy(o => o.PaymentMethod)
                .Select(g => new { Method = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.TotalSales = totalSales;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TopProducts = topProducts;
            ViewBag.PaymentMethods = paymentMethods;

            return View(orders);
        }
    }
}