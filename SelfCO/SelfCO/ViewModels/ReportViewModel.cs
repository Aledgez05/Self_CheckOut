namespace SelfCheckoutSystem.ViewModels
{
    public class ReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReportType { get; set; } // Daily, Monthly

        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }

        public Dictionary<string, decimal> SalesByPaymentMethod { get; set; }
        public List<TopProductViewModel> TopProducts { get; set; }
        public List<TopCategoryViewModel> TopCategories { get; set; }
    }

    public class TopProductViewModel
    {
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class TopCategoryViewModel
    {
        public string CategoryName { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}