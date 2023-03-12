using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HW02.BussinessContext
{
    public class Category
    {
        private readonly int _id;
        private string _name;

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

        public Category(int id, string name)
        {
            _id = id;
            _name = name;
        }
    }
}
