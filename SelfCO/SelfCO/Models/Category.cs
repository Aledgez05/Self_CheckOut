using System.ComponentModel.DataAnnotations;
namespace SelfCheckoutSystem.Models
{
    public class Category
    {
        [Key] public int CategoryId { get; set; }
        [Required][StringLength(100)] public string Name { get; set; }
        [StringLength(300)] public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        //nav
        public ICollection<Product> Products { get; set; }

    }
}
