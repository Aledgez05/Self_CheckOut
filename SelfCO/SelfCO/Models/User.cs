using System.ComponentModel.DataAnnotations;

namespace SelfCheckoutSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; } // Admin, Staff, Customer

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public ICollection<Order> Orders { get; set; }
    }
}
