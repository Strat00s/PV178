﻿using HW02.AnalyticalDataContext.DB;
using HW02.BussinessContext;
using HW02.Helpers;

namespace HW02.AnalyticalDataContext
{
    public class AnalyticalDataListener
    {
        private readonly AnalyticalDBContext _db;
        private readonly List<AnalyticData> _data;
        public AnalyticalDataListener(AnalyticalDBContext analyticalDBContext)
        {
            _db = analyticalDBContext;
            _data = _db.ReadAnalyticalData();
        }

        public void HandleEvent(Object? sender, LogEventArgs e)// OpCode opCode, bool status, Category? entity = null, string? msg = null)
        {
            if (!e.Status || e.Entity == null)
                return;

            switch (e.OpCode)
            {
                case OpCode.ADD_CATG: _data.Add(new AnalyticData(e.Entity.Id, e.Entity.Name, 0)); break;                        //add new entry
                case OpCode.UPD_CATG: _data.Find(item => item.CategoryId == e.Entity.Id).CategoryName = e.Entity.Name; break;   //find category by id and update it's name
                case OpCode.DEL_CATG: _data.RemoveAll(item => item.CategoryId == e.Entity.Id); break;                         //remove entry


                //always check if entity is a product
                //add product to correct entry
                case OpCode.ADD_PROD:
                    if (e.Entity is Product product1)
                        _data.Find(item => item.CategoryId == product1.CategoryId)?.AddProduct(product1.Id);
                    break;

                //remove product from old entry and add it to the new one
                case OpCode.UPD_PROD:
                    if (e.Entity is not Product product2)
                        break;
                    _data.Find(item => item.HasProduct(product2.Id))?.RemoveProduct(product2.Id);
                    _data.Find(item => item.CategoryId == product2.CategoryId)?.AddProduct(product2.Id);
                    break;

                //remove product from entry
                case OpCode.DEL_PROD:
                    if (e.Entity is Product product3)
                        _data.Find(item => item.CategoryId == product3.CategoryId)?.RemoveProduct(product3.Id);
                    break;
                default: return;
            }

            _db.SaveAnalyticalData(_data);  //save it
        }
    }
}
