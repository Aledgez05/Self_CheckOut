using System.ComponentModel.DataAnnotations;

namespace SelfCheckoutSystem.ViewModels
{
    public class CheckoutViewModel
    {
        public CartViewModel Cart { get; set; }

        [Required(ErrorMessage = "Please select a payment method")]
        public string PaymentMethod { get; set; }

        // For cash payments
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal? AmountReceived { get; set; }

        public decimal? ChangeGiven { get; set; }

        public List<string> AvailablePaymentMethods { get; set; } = new()
        {
            "Cash",
            "Card"
        };
    }
}
