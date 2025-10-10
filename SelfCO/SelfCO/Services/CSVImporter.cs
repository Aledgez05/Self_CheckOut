using SelfCheckoutSystem.Data;
using SelfCheckoutSystem.Models;

namespace SelfCheckoutSystem.Services
{
    public class CSVImporter
    {
        private readonly ApplicationDbContext _context;

        public CSVImporter(ApplicationDbContext context)
        {
            _context = context;
        }

        public void ImportProducts(string filePath)
        {
            var lines = File.ReadAllLines(filePath).Skip(1); 

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                if (parts.Length < 5) continue;

                var code = parts[4].Trim(); // code column
                var name = parts[1].Trim(); // name column
                var price = decimal.Parse(parts[0].Trim()); // price column
                var categoryName = parts[2].Trim(); // category column
                var brand = parts[3].Trim(); // brand column

                var category = _context.Categories.FirstOrDefault(c => c.Name == categoryName);
                if (category == null)
                {
                    category = new Category { Name = categoryName, IsActive = true };
                    _context.Categories.Add(category);
                    _context.SaveChanges();
                }

                if (!_context.Products.Any(p => p.Code == code))
                {
                    var product = new Product
                    {
                        Code = code,
                        Name = name,
                        Brand = brand,
                        Price = price,
                        CategoryId = category.CategoryId,
                        IsActive = true
                    };
                    _context.Products.Add(product);
                }
            }

            _context.SaveChanges();
        }
    }
}