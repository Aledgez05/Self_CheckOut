using System.ComponentModel.DataAnnotations;

namespace SelfCheckoutSystem.ViewModels
{
    public class CheckoutViewModel
    {
        public CartViewModel Cart { get; set; }

        [Required(ErrorMessage = "Please select a payment method")]
        public string PaymentMethod { get; set; }

        [EmailAddress]
        public string Email { get; set; } // Optional for receipt

        public List<string> AvailablePaymentMethods { get; set; } = new()
        {
            "Cash",
            "Credit Card",
            "Debit Card",
            "Mobile Payment"
        };
    }
}