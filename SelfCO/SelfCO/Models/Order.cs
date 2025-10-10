using SelfCO.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfCheckoutSystem.Models
{
    public class Order
    {
        [Key] public int OrderId { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")] public User User { get; set; }
        [Required] public DateTime OrderDate { get; set; } = DateTime.Now;
        [Required][Column(TypeName = "decimal(18,2)")] public int TotalAmount { get; set; }
        [Required][StringLength(50)] public string PaymentMethod { get; set; }//cash, card
        [Required][StringLength(50)] public string Status { get; set; }//completed, canceled

        // For cash payments
        [Column(TypeName = "decimal(18,2)")]public decimal? AmountReceived { get; set; }

        [Column(TypeName = "decimal(18,2)")]public decimal? ChangeGiven { get; set; }

        public int TotalItems { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
