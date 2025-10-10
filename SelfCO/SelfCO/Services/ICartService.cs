using SelfCheckoutSystem.ViewModels;

namespace SelfCheckoutSystem.Services
{
    public interface ICartService
    {
        CartViewModel GetCart();
        void AddToCart(int productId, int quantity = 1);
        void RemoveFromCart(int productId);
        void UpdateQuantity(int productId, int quantity);
        void ClearCart();
    }
}