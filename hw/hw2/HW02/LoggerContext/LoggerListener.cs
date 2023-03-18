/* I probably missunderstood how this class is suppose to work...
 */

using HW02.BussinessContext;
using HW02.Helpers;
using HW02.LoggerContext.DB;

namespace HW02
{
    public class LoggerListener
    {

        private readonly LoggerDBContext _db;
        public LoggerListener(LoggerDBContext loggerDBContext)
        {
            _db = loggerDBContext;
        }

        //handle event and create string that will be logged
        public void HandleEvent(Object? sender, LogEventArgs e)//OpCode opCode, bool status, Category? entity = null, string? msg = null)
        {
            string log = DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]") + " ";  //add time

            //write appropriate command
            switch (e.OpCode)
            {
                case OpCode.EXIT: log += "Exit"; _db.WriteLog(log); return;
                case OpCode.HELP: log += "Help"; _db.WriteLog(log); return;
                
                case OpCode.GET_BY_CATG:
                case OpCode.LST_PROD: log += "Get; Product; ";    break;
                case OpCode.ADD_PROD: log += "Add; Product; ";    break;
                case OpCode.DEL_PROD: log += "Delete; Product; "; break;
                case OpCode.UPD_PROD: log += "Update; Product; "; break;
                
                case OpCode.ADD_CATG: log += "Add; Category; ";    break;
                case OpCode.DEL_CATG: log += "Delete; Category; "; break;
                case OpCode.UPD_CATG: log += "Update; Category; "; break;
                case OpCode.LST_CATG: log += "Get; Category; ";    break;

                default: log += "Other; " + e.Message ?? ""; _db.WriteLog(log); return;
            }

            //write message on fail
            if (!e.Success)
            {
                log += "Failure; " + e.Message ?? "";
                _db.WriteLog(log);
                return;
            }

            log += "Success; ";
            
            //get commands don't have entity
            if (e.Entity == null)
            {
                _db.WriteLog(log);
                return;
            }

            //write appropriate info
            log += e.Entity.Id + "; " + e.Entity.Name + "; ";
            if (e.Entity is Product product)
                log += product.CategoryId;
            else
                log += e.Entity.Id;

            _db.WriteLog(log);
        }
    }
}
