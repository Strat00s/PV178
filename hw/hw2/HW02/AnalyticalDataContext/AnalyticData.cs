/* Hopefully correct analytic data (json seems to be fine)
 */

namespace HW02.AnalyticalDataContext
{
    public class AnalyticData
    {
        private int _productCount;              //number of products in category
        private readonly List<int> _productIds; //stores IDs of products for easy updating

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
