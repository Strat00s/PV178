using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW02.BussinessContext
{
    public class Product
    {
        private readonly int _id;
        private string _name;
        private int _categoryId;
        private decimal _price;

        public int Id
        {
            get 
            { 
                return _id;
            }
        }

        public string Name
        {
            get
            { 
                return _name;
            }
            set
            { 
                _name = value; 
            }
        }

        public int CategoryId
        {
            get 
            { 
                return _categoryId; 
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("CategoryId must be a positive integer");
                _categoryId = value;
            }
        }

        public decimal Price
        {
            get { return _price; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Price must be a positive decimal");
                }
                _price = value;
            }
        }

        public Product(int id, string name, int categoryId, decimal price)
        {
            _id = id;
            _name = name;
            _categoryId = categoryId;
            _price = price;
        }
    }
}
