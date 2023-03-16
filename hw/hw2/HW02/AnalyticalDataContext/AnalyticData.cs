using HW02.BussinessContext;

namespace HW02.AnalyticalDataContext
{
    public class AnalyticData
    {
        private int _productCount;
        private readonly List<int> _productIds;

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductCount { get { return _productCount; } }

        public AnalyticData(int categoryId, string name, int productCount)
        {
            CategoryId    = categoryId;
            CategoryName  = name;
            _productCount = productCount;
            _productIds   = new List<int>();
        }

        //have to be seperate methods. Otherwise json serialize would save them
        public void AddProduct(int productId)
        {
            _productIds.Add(productId);
            _productCount++;
        }
        public void RemoveProduct(int productId)
        {
            _productIds.Remove(productId);
            _productCount--;
        }

        public bool HasProduct(int productId)
        {
            return _productIds.Contains(productId);
        }
    }
}
