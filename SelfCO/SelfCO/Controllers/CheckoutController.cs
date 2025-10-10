using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfCheckoutSystem.Data;
using SelfCheckoutSystem.Models;
using SelfCheckoutSystem.Services;
using SelfCheckoutSystem.ViewModels;

namespace SelfCheckoutSystem.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;

        public CheckoutController(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        // GET: Checkout/Scan
        public IActionResult Scan()
        {
            var cart = _cartService.GetCart();
            return View(cart);
        }

        // POST: Checkout/ScanBarcode
        [HttpPost]
        public IActionResult ScanBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return Json(new { success = false, message = "Invalid barcode" });

            var product = _context.Products
                .FirstOrDefault(p => p.Code == barcode && p.IsActive);

            if (product == null)
                return Json(new { success = false, message = "Product not found" });

            if (product.Stock <= 0)
                return Json(new { success = false, message = "Product out of stock" });

            _cartService.AddToCart(product.ProductId);

            return Json(new
            {
                success = true,
                message = $"Added {product.Name}",
                product = new
                {
                    name = product.Name,
                    price = product.Price
                }
            });
        }

        // GET: Checkout/Cart
        public IActionResult Cart()
        {
            var cart = _cartService.GetCart();
            return View(cart);
        }

        // POST: Checkout/UpdateCart
        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            _cartService.UpdateQuantity(productId, quantity);
            return RedirectToAction(nameof(Cart));
        }

        // POST: Checkout/RemoveItem
        [HttpPost]
        public IActionResult RemoveItem(int productId)
        {
            _cartService.RemoveFromCart(productId);
            return RedirectToAction(nameof(Cart));
        }

        // GET: Checkout/Payment
        public IActionResult Payment()
        {
            var cart = _cartService.GetCart();

            if (!cart.Items.Any())
                return RedirectToAction(nameof(Scan));

            var viewModel = new CheckoutViewModel { Cart = cart };
            return View(viewModel);
        }

        // POST: Checkout/ProcessPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessPayment(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Payment", model);

            var cart = _cartService.GetCart();

            if (!cart.Items.Any())
                return RedirectToAction(nameof(Scan));

            // Create order
            var order = new Order
            {
                OrderDate = DateTime.Now,
                TotalAmount = (int)cart.TotalAmount,
                PaymentMethod = model.PaymentMethod,
                Status = "Completed"
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // Create order items
            foreach (var item in cart.Items)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                };
                _context.OrderItems.Add(orderItem);

                // Update stock
                var product = _context.Products.Find(item.ProductId);
                if (product != null)
                    product.Stock -= item.Quantity;
            }

            // Create payment record
            var payment = new Payment
            {
                OrderId = order.OrderId,
                Amount = cart.TotalAmount,
                Method = model.PaymentMethod,
                TransactionId = Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper(),
                PaymentDate = DateTime.Now,
                IsSuccessful = true
            };
            _context.Payments.Add(payment);

            _context.SaveChanges();

            // Clear cart
            _cartService.ClearCart();

            return RedirectToAction(nameof(Confirmation), new { id = order.OrderId });
        }

        // GET: Checkout/Confirmation
        public IActionResult Confirmation(int id)
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
    }
}