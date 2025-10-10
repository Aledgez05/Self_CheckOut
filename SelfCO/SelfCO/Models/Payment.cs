using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfCheckoutSystem.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string Method { get; set; }

        [StringLength(200)]
        public string TransactionId { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public bool IsSuccessful { get; set; } = true;
    }
}