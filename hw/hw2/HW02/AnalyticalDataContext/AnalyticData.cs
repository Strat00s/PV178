using HW02.BussinessContext;

namespace HW02.AnalyticalDataContext
{
    public class AnalyticData
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }

        public AnalyticData(int categoryId, string name, int productCount)
        {
            CategoryId = categoryId;
            CategoryName = name;
            ProductCount = productCount;
        }
    }
}
