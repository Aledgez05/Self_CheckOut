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
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Code == barcode && p.IsActive);

            if (product == null)
                return Json(new { success = false, message = "Product not found" });

            _cartService.AddToCart(product.ProductId);

            return Json(new
            {
                success = true,
                message = $"Added {product.Name}",
                product = new
                {
                    name = product.Name,
                    brand = product.Brand,
                    price = product.Price
                }
            });
        }

        // GET: Checkout/Cart
        public IActionResult Cart()
        {
            var cart = _cartService.GetCart();
            if (!cart.Items.Any())
            {
                return RedirectToAction(nameof(Scan));
            }
            return View(cart);
        }

        // POST: Checkout/RemoveItem
        [HttpPost]
        public IActionResult RemoveItem(int productId)
        {
            _cartService.RemoveFromCart(productId);
            return RedirectToAction(nameof(Cart));
        }

        // POST: Checkout/CancelOrder
        [HttpPost]
        public IActionResult CancelOrder()
        {
            _cartService.ClearCart();
            return RedirectToAction("Index", "Home");
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
            var cart = _cartService.GetCart();

            if (!cart.Items.Any())
                return RedirectToAction(nameof(Scan));

            // Validate cash payment
            if (model.PaymentMethod == "Cash")
            {
                if (!model.AmountReceived.HasValue || model.AmountReceived < cart.TotalAmount)
                {
                    ModelState.AddModelError("AmountReceived", "Amount received must be greater than or equal to total");
                    model.Cart = cart;
                    return View("Payment", model);
                }
                model.ChangeGiven = model.AmountReceived.Value - cart.TotalAmount;
            }

            if (!ModelState.IsValid)
            {
                model.Cart = cart;
                return View("Payment", model);
            }

            // Create order
            var order = new Order
            {
                OrderDate = DateTime.Now,
                TotalAmount = (int)cart.TotalAmount,
                TotalItems = cart.TotalItems,
                PaymentMethod = model.PaymentMethod,
                Status = "Completed",
                AmountReceived = model.AmountReceived,
                ChangeGiven = model.ChangeGiven
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
            }

            _context.SaveChanges();

            // Clear cart
            _cartService.ClearCart();

            return RedirectToAction(nameof(Receipt), new { id = order.OrderId });
        }

        // GET: Checkout/Receipt
        public IActionResult Receipt(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}