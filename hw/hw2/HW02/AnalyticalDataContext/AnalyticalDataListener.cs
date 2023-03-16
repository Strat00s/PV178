using HW02.AnalyticalDataContext.DB;
using HW02.BussinessContext;

namespace HW02.AnalyticalDataContext
{
    public class AnalyticalDataListener
    {
        private AnalyticalDBContext _db;
        private List<AnalyticData> _data;
        public AnalyticalDataListener(AnalyticalDBContext analyticalDBContext)
        {
            _db = analyticalDBContext;
            _data = _db.ReadAnalyticalData();
        }

        public void HandleEvent(OpCode opCode, bool status, Category? entity = null, string? msg = null)
        {
            if (!status || entity == null)
                return;

            switch (opCode)
            {
                case OpCode.ADD_CATG:
                    AnalyticData data = new AnalyticData(entity.Id, entity.Name, 0);
                    _data.Add(data);
                    break;
                case OpCode.UPD_CATG: break;
                case OpCode.DEL_CATG:
                    _data.RemoveAll(data => data.CategoryId == entity.Id);
                    break;
                case OpCode.ADD_PROD: break;
                case OpCode.UPD_PROD: break;
                case OpCode.DEL_PROD: break;
                default: return;
            }

            _db.SaveAnalyticalData(_data);
        }
    }
}
