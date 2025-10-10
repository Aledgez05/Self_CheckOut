namespace SelfCheckoutSystem.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal TotalAmount => Items.Sum(i => i.Subtotal);
        public int TotalItems => Items.Sum(i => i.Quantity);
    }

    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
    }
}