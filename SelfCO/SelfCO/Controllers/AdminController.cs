using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfCheckoutSystem.Data;
using SelfCheckoutSystem.Models;
using SelfCheckoutSystem.ViewModels;

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
            return View();
        }

        // Search Ticket
        public IActionResult SearchTicket()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SearchTicket(int ticketId)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefault(o => o.OrderId == ticketId);

            if (order == null)
            {
                ViewBag.Error = "Ticket not found";
                return View();
            }

            return View("TicketDetails", order);
        }

        // Products Management
        public IActionResult Products()
        {
            var products = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .ToList();

            return View(products);
        }

        public IActionResult CreateProduct()
        {
            ViewBag.Categories = _context.Categories.Where(c => c.IsActive).ToList();
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

            ViewBag.Categories = _context.Categories.Where(c => c.IsActive).ToList();
            return View(product);
        }

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

        // Reports
        public IActionResult Reports(string reportType = "Daily", DateTime? startDate = null, DateTime? endDate = null)
        {
            if (reportType == "Daily")
            {
                startDate = startDate ?? DateTime.Today;
                endDate = startDate;
            }
            else // Monthly
            {
                if (!startDate.HasValue)
                {
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    endDate = startDate.Value.AddMonths(1).AddDays(-1);
                }
            }

            var orders = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .Where(o => o.OrderDate.Date >= startDate && o.OrderDate.Date <= endDate
                            && o.Status == "Completed")
                .ToList();

            var viewModel = new ReportViewModel
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                ReportType = reportType,
                TotalSales = orders.Sum(o => o.TotalAmount),
                TotalOrders = orders.Count,

                // Sales by payment method
                SalesByPaymentMethod = orders
                    .GroupBy(o => o.PaymentMethod)
                    .ToDictionary(g => g.Key, g => (decimal)g.Sum(o => o.TotalAmount)),

                // Top 10 products
                TopProducts = orders
                    .SelectMany(o => o.OrderItems)
                    .GroupBy(oi => new { oi.Product.ProductId, oi.Product.Name })
                    .Select(g => new TopProductViewModel
                    {
                        ProductName = g.Key.Name,
                        QuantitySold = g.Sum(oi => oi.Quantity),
                        TotalRevenue = g.Sum(oi => oi.Subtotal)
                    })
                    .OrderByDescending(x => x.QuantitySold)
                    .Take(10)
                    .ToList(),

                // Top 3 categories
                TopCategories = orders
                    .SelectMany(o => o.OrderItems)
                    .GroupBy(oi => oi.Product.Category.Name)
                    .Select(g => new TopCategoryViewModel
                    {
                        CategoryName = g.Key,
                        QuantitySold = g.Sum(oi => oi.Quantity),
                        TotalRevenue = g.Sum(oi => oi.Subtotal)
                    })
                    .OrderByDescending(x => x.QuantitySold)
                    .Take(3)
                    .ToList()
            };

            return View(viewModel);
        }
    }
}